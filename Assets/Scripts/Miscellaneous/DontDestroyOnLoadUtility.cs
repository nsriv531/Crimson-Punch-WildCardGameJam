using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using UnityEngine;

public class DontDestroyOnLoadUtility : MonoBehaviour
{
    /*
    private NetworkObject _networkObject;
    private NetworkObject networkObject
    {
        get
        {
            if (_networkObject == null) _networkObject = transform.root.GetComponentInChildren<NetworkObject>();
            return _networkObject;
        }
    }
    */

    void Awake()
    {
        //if (networkObject != null)
        //    if (!networkObject.IsOwner) return;

        DontDestroyOnLoad(transform.root.gameObject);
    }

    void OnEnable()
    {
        //if (networkObject != null)
        //    if (!networkObject.IsOwner) return;

        DontDestroyOnLoad(transform.root.gameObject);
    }
}
