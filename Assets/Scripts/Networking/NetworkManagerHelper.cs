using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(NetworkManager))]
public class NetworkManagerHelper : MonoBehaviour
{
    [SerializeField] private bool _debug = false;

    [HideInInspector] public bool debug { get => _debug; }

    private NetworkManager _networkManager;
    [HideInInspector]
    public NetworkManager networkManager
    {
        get
        {
            if (_networkManager == null) _networkManager = GetComponent<NetworkManager>();
            return _networkManager;
        }
    }

    private NetworkRelay _networkRelay;
    [HideInInspector]
    public NetworkRelay networkRelay
    {
        get
        {
            if (_networkRelay == null) _networkRelay = networkManager.GetComponentInChildren<NetworkRelay>(true);
            return _networkRelay;
        }
    }

    public void StartHost()
    {
        //if (debug) networkRelay.onConnected.AddListener(() => SetLocalPlayerActive(true));
        networkRelay.CreateRelay("Host");
    }
    public void StartServer()
    {
        networkRelay.CreateRelay("Server");
    }
    public void StartClient()
    {
        //if (debug) networkRelay.onConnected.AddListener(() => SetLocalPlayerActive(true));
        networkManager.StartClient();
    }

    /*
    public void SetLocalPlayerActive(bool value)
    {
        Character localCharacter = NetworkManager.Singleton.GetCharacter(NetworkManager.Singleton.LocalClientId);
        Debug.Log(localCharacter);
        if (localCharacter != null) localCharacter.gameObject.SetActive(value);
    }
    */
}
