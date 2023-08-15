using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PUNCorpse : MonoBehaviourPunCallbacks
{
    [PunRPC]
    public void SetOrientation(Vector3 normal)
    {
        transform.up = normal;
    }
}
