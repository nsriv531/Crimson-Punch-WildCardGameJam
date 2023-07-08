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
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) targeting.enabled = false;
        else
        {
            // Connect targeting events
            if (IsOwner && !IsHost)
            {
                targeting.onTargetSet.AddListener(OnTargetSet);
                targeting.onTargetCleared.AddListener(OnTargetCleared);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        // Disconnect targeting events
        if (IsOwner && !IsHost)
        {
            targeting.onTargetSet.RemoveListener(OnTargetSet);
            targeting.onTargetCleared.RemoveListener(OnTargetCleared);
        }
        base.OnNetworkDespawn();
    }

    private void OnTargetSet()
    {
        NetworkObject networkObject = targeting.target.GetComponentInParent<NetworkObject>(true);
        if (networkObject == null) networkObject = targeting.target.GetComponentInChildren<NetworkObject>(true);
        if (networkObject == null) throw new System.Exception("Cannot set a target to something which has no NetworkObject.");
        if (IsServer) SetTargetClientRpc(targeting.target.clientId);
        else if (IsOwner) SetTargetServerRpc(targeting.target.clientId);
    }

    private void OnTargetCleared()
    {
        if (IsServer) ClearTargetClientRpc();
        else if (IsOwner) ClearTargetServerRpc();
    }


    /// <summary>
    /// Called by clients only, run by the server only.
    /// </summary>
    [ServerRpc]
    public void SetTargetServerRpc(ulong targetClientId)
    {
        targeting.SetTarget(NetworkManager.Singleton.GetCharacter(targetClientId).gameObject);
        SetTargetClientRpc(targetClientId);
    }

    /// <summary>
    /// Called by the server only, run by the clients only.
    /// </summary>
    [ClientRpc]
    public void SetTargetClientRpc(ulong targetClientId)
    {
        if (!IsOwner)
        {
            //Debug.Log("ClientRpc "+ character.gameObject.name + " set target to " + targetClientId);
            targeting.SetTarget(NetworkManager.Singleton.GetCharacter(targetClientId).gameObject);
        }
    }

    /// <summary>
    /// Called by clients only, run by the server only.
    /// </summary>
    [ServerRpc]
    public void ClearTargetServerRpc()
    {
        targeting.ClearTarget();
        ClearTargetClientRpc();
    }

    /// <summary>
    /// Called by the server only, run by the clients only.
    /// </summary>
    [ClientRpc]
    public void ClearTargetClientRpc()
    {
        if (!IsOwner)
        {
            targeting.ClearTarget();
        }
    }
}
