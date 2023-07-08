using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NetworkRelay : MonoBehaviour
{
    [SerializeField] private string _joinCode;

    public UnityEvent onSignedIn = new UnityEvent();
    public UnityEvent onSignedOut = new UnityEvent();
    public UnityEvent onStart = new UnityEvent();
    public UnityEvent onInitialized = new UnityEvent();
    public UnityEvent onCreationStarted = new UnityEvent();
    public UnityEvent onAllocationCreated = new UnityEvent();
    public UnityEvent onGetJoinCode = new UnityEvent();
    public UnityEvent onError = new UnityEvent();
    public UnityEvent onJoinAllocation = new UnityEvent();
    public UnityEvent onAllocationJoined = new UnityEvent();
    public UnityEvent onConnected = new UnityEvent();
    public UnityEvent onDisconnected = new UnityEvent();

    [Header("Debugging")]
    [ReadOnly, SerializeField] private bool _signedIn = false;
    [ReadOnly, SerializeField] private bool _connected = false;
    [ReadOnly, SerializeField] private Allocation _allocation;
    [ReadOnly, SerializeField] private JoinAllocation _joinAllocation;

    public bool signedIn { get => _signedIn; private set => _signedIn = value; }
    public bool connected { get => _connected; private set => _connected = value; }
    public Allocation allocation { get => _allocation; private set => _allocation = value; }
    public JoinAllocation joinAllocation { get => _joinAllocation; private set => _joinAllocation = value; }

    private const string transportService = "dtls";
    private const int maxConnections = 10;



    [HideInInspector] public string joinCode { get => _joinCode; private set => _joinCode = value; }

    private UnityTransport _unityTransport;
    private UnityTransport unityTransport
    {
        get
        {
            if (_unityTransport == null) _unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (_unityTransport == null) throw new System.Exception("There is no UnityTransport script attached to the NetworkManager");
            return _unityTransport;
        }
    }

    // Start is called before the first frame update
    private async void Start()
    {
        onStart.Invoke();
        await UnityServices.InitializeAsync();

        onInitialized.Invoke();

        AuthenticationService.Instance.SignedOut += () =>
        {
            signedIn = false;
            Debug.Log("Signed out " + AuthenticationService.Instance.PlayerId);
            onSignedOut.Invoke();
        };

        AuthenticationService.Instance.SignedIn += () =>
        {
            signedIn = true;
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            onSignedIn.Invoke();
        };
    }

    public async void SignIn()
    {
        if (!signedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public void SignOut()
    {
        if (signedIn)
            AuthenticationService.Instance.SignOut();
    }

    private RelayServerData GetRelayServerData(Allocation allocation) => new RelayServerData(allocation, transportService);
    private RelayServerData GetRelayServerData(JoinAllocation allocation) => new RelayServerData(allocation, transportService);
    private void SetRelayServerData(Allocation allocation) => unityTransport.SetRelayServerData(GetRelayServerData(allocation));
    private void SetRelayServerData(JoinAllocation allocation) => unityTransport.SetRelayServerData(GetRelayServerData(allocation));

    public async void CreateRelay(string startType = "Host")
    {
        try
        {
            onCreationStarted.Invoke();

            // Create an allocation on the relay
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            onAllocationCreated.Invoke();

            // Get a join code to join the allocation. This code can be sent to other people to let them connect to the same relay.
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join Code = " + joinCode);

            onGetJoinCode.Invoke();

            SetRelayServerData(allocation);

            if (startType == "Host") NetworkManager.Singleton.StartHost();
            else if (startType == "Server") NetworkManager.Singleton.StartServer();
            onConnected.Invoke();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            onError.Invoke();
        }
    }

    public async void JoinRelay(string joinCode = null)
    {
        if (joinCode == null) joinCode = this.joinCode;

        try
        {
            Debug.Log("Joining relay with code " + joinCode);
            onJoinAllocation.Invoke();
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            SetRelayServerData(joinAllocation);
            onAllocationJoined.Invoke();

            NetworkManager.Singleton.StartClient();

            // Spawn a player for the client (why isn't this done automatically anymore??)


            //onConnected.Invoke();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            onError.Invoke();
        }
    }

    public async void JoinRelay(TMPro.TMP_InputField inputField) => JoinRelay(inputField.text);

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        onDisconnected.Invoke();
        //NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
    }
}




#if UNITY_EDITOR
[CustomEditor(typeof(NetworkRelay))]
public class NetworkRelayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool wasEnabled = GUI.enabled;

        NetworkRelay script = target as NetworkRelay;
        EditorGUILayout.BeginHorizontal();
        {
            GUI.enabled = script.signedIn;
            if (GUILayout.Button("Create Relay")) script.CreateRelay();
            if (GUILayout.Button("Join Relay")) script.JoinRelay();
            GUI.enabled = wasEnabled;
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif