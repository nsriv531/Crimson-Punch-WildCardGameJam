using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class JoinCodeListener : MonoBehaviour
{
    [Header("Debugging")]
    [ReadOnly, SerializeField] private string _joinCode;

    [HideInInspector]
    public string joinCode
    {
        get => _joinCode;
        private set
        {
            if (_joinCode != value)
            {
                _joinCode = value;
                onJoinCodeChanged.Invoke();
            }
        }
    }

    public UnityEvent onJoinCodeChanged = new UnityEvent();


    private NetworkRelay _networkRelay;
    [HideInInspector]
    public NetworkRelay networkRelay
    {
        get
        {
            if (_networkRelay == null)
            {
                if (NetworkManager.Singleton == null) throw new System.Exception("Failed to find the NetworkManager singleton.");
                _networkRelay = NetworkManager.Singleton.GetComponentInChildren<NetworkRelay>(true);
            }
            if (_networkRelay == null) throw new System.Exception("Failed to find the NetworkRelay in any children of the NetworkManager");
            return _networkRelay;
        }
    }

    private bool _tryOnStart = false;

    void Awake()
    {
        try
        {
            networkRelay.onGetJoinCode.AddListener(() => joinCode = networkRelay.joinCode);
        }
        catch (System.Exception)
        {
            _tryOnStart = true;
        }
    }

    void Start()
    {
        if (_tryOnStart) networkRelay.onGetJoinCode.AddListener(() => joinCode = networkRelay.joinCode);
    }


    public void SetInputFieldText(TMPro.TMP_InputField inputField) => inputField.text = joinCode;
}
