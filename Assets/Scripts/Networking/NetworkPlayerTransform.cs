using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Synchronizes the player's transform over the network
/// </summary>

[RequireComponent(typeof(Character))]
public class NetworkPlayerTransform : NetworkBehaviour
{
    /// <summary>
    /// Ticks per second
    /// </summary>
    private uint tickRate;

    private Character _character;
    [HideInInspector] public Character character
    {
        get
        {
            if(_character == null) _character = GetComponent<Character>();
            return _character;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            tickRate = NetworkManager.Singleton.NetworkTickSystem.TickRate;
            InvokeRepeating(nameof(OnNetworkTick), 0f, 1f / tickRate);
        }
    }

    /// <summary>
    /// Called on every network update tick
    /// </summary>
    private void OnNetworkTick()
    {
        // Synchronize this transform with the clients
        if (IsServer) SynchronizeClientRpc(transform.position, transform.rotation, transform.localScale);
        else SynchronizeServerRpc(transform.position, transform.rotation, transform.localScale, character.clientId);
    }

    /// <summary>
    /// Called by server only, run by clients only.
    /// </summary>
    [ClientRpc]
    private void SynchronizeClientRpc(Vector3 position, Quaternion rotation, Vector3 scale, ClientRpcParams clientRpcParams = default)
    {

        
        if (clientRpcParams.Send.TargetClientIds != null)
        {
            if (!clientRpcParams.Send.TargetClientIds.Contains(character.clientId)) return;
        }
        transform.SetPositionAndRotation(position, rotation);
        transform.localScale = scale;
    }

    /// <summary>
    /// Called by clients only, run by the server only.
    /// </summary>
    [ServerRpc]
    private void SynchronizeServerRpc(Vector3 position, Quaternion rotation, Vector3 scale, ulong clientId)
    {
        transform.SetPositionAndRotation(position, rotation);
        transform.localScale = scale;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        SynchronizeClientRpc(position, rotation, scale, clientRpcParams);
    }
}
