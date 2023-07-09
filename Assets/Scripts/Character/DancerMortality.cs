using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;

public class DancerMortality : MonoBehaviourPunCallbacks
{
    public bool isPlayer;
    public GameObject deathEffect;
    
    public void Kill()
    {
        photonView.RPC("KillRPC", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void KillRPC()
    {
        PhotonNetwork.Destroy(gameObject);
        // TODO: play death effect

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
