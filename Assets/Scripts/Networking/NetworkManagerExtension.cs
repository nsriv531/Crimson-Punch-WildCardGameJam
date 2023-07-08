using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public static class NetworkManagerExtension
{
    public static NetworkClient GetNetworkClient(this NetworkManager networkManager, ulong clientId)
    {
        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)) throw new System.Exception("Client with ID " + clientId + " is not connected");
        return NetworkManager.Singleton.ConnectedClients[clientId];
    }

    public static Character GetCharacter(this NetworkManager networkManager, ulong clientId)
    {
        foreach (NetworkObject networkObject in UnityEngine.Object.FindObjectsOfType<NetworkObject>(true))
        {
            if (networkObject.OwnerClientId == clientId) return networkObject.GetComponent<Character>();
        }
        return null;
    }

    public static System.Action<ulong> AddOnClientDisconnectCallbackOneTime(this NetworkManager networkManager, System.Action<ulong> action)
    {
        System.Action<ulong> newAction = null;
        newAction = new System.Action<ulong>(delegate (ulong dummy) { action(dummy); networkManager.OnClientDisconnectCallback -= newAction; });
        networkManager.OnClientDisconnectCallback += newAction;
        return newAction;
    }

    public static System.Action<ulong> AddOnClientConnectedCallbackOneTime(this NetworkManager networkManager, System.Action<ulong> action)
    {
        System.Action<ulong> newAction = null;
        newAction = new System.Action<ulong>(delegate (ulong dummy) { action(dummy); networkManager.OnClientConnectedCallback -= newAction; });
        networkManager.OnClientConnectedCallback += newAction;
        return newAction;
    }


}
