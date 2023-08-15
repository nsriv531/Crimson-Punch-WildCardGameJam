using System.Collections;
using System.Collections.Generic;using Photon.Pun;
using UnityEngine;

public class DestroyOnGameEnd : MonoBehaviour, IGameEventListener
{
    public bool photonDestroy = false;
    void OnEnable()
    {
        GameManager.RegisterEventListener(this);
    }

    void OnDisable()
    {
        GameManager.UnregisterEventListener(this);
    }
    
    public void OnEventRaised(string eventType, object data)
    {
        if (eventType == "GameEnd")
        {
            if (photonDestroy)
            {
                if(GetComponent<PhotonView>().IsMine)
                    PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
