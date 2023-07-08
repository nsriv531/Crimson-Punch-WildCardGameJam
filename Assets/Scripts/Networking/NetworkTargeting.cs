using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Character))]
public class NetworkTargeting : NetworkBehaviour
{
    private Character _character;
    [HideInInspector] public Character character
    {
        get
        {
            if (_character == null) _character = GetComponent<Character>();
            return _character;
        }
    }

    private Targeting _targeting;
    [HideInInspector] public Targeting targeting
    {
        get
        {
            if (_targeting == null) _targeting = character.GetComponentInChildren<Targeting>(true);
            return _targeting;
        }
    }

    // Awake -> OnEnable -> OnNetworkSpawn -> Start
    void Start()
    {
        if (IsClient)
        {
            targeting.onTargetSet.AddListener(() => SetTargetServerRpc(OwnerClientId, targeting.target.OwnerClientId));
            targeting.onTargetCleared.AddListener(() => ClearTargetServerRpc(OwnerClientId));
        }
    }


    /// <summary>
    /// Called by clients only, run by the server only.
    /// </summary>
    [ServerRpc]
    public void SetTargetServerRpc(ulong clientId, ulong targetId)
    {
        // Update the server's state. targeting is the script attached to the object on the server
        // that corresponds with the object that originally called this method.
        targeting.SetTarget(Character.clientIds[targetId]);

        // Update the clients' states
        SetTargetClientRpc(clientId, targetId);
    }

    /// <summary>
    /// Called by the server only, run by the clients only.
    /// </summary>
    [ClientRpc]
    public void SetTargetClientRpc(ulong clientId, ulong targetId)
    {
        // clientId = the client that sent the message to the server

        // Don't update the client that already set their target
        if (OwnerClientId == clientId) return;

        // Update all the other clients' corresponding game objects that represent the game object who
        // originally called this method.
        targeting.SetTarget(Character.clientIds[targetId]);
    }

    /// <summary>
    /// Called by clients only, run by the server only.
    /// </summary>
    [ServerRpc]
    public void ClearTargetServerRpc(ulong clientId)
    {
        // Update the server's state
        targeting.ClearTarget();

        // Update the clients' states
        ClearTargetClientRpc(clientId);
    }

    /// <summary>
    /// Called by the server only, run by the clients only.
    /// </summary>
    [ClientRpc]
    public void ClearTargetClientRpc(ulong clientId)
    {
        // clientId = the client that sent the message to the server
        
        // Don't update the client who orignally called this method.
        if (OwnerClientId == clientId) return;

        // Update all the other clients
        targeting.ClearTarget();
    }


}
