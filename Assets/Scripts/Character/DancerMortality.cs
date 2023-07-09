using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class DancerMortality : MonoBehaviourPunCallbacks
{
    public bool isPlayer;

    public UnityEvent onDeath = new UnityEvent();

    public void Kill()
    {
        photonView.RPC("KillRPC", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void KillRPC()
    {
        PhotonNetwork.Destroy(gameObject);
        // TODO: play death effect

        onDeath.Invoke(); // Does this go here?

        if (photonView.IsMine)
        {
            // TODO: move camera to "spectator" view overlooking the map
        }

        if (isPlayer)
        {
            GameManager.instance.PlayerKilled();
        }
    }
}
