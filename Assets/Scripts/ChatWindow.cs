using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviour
{
    public static ChatWindow instance;
    
    public Transform chatWindow;
    public GameObject chatMessagePrefab;
    
    public int maxMessages = 10;

    private PhotonView _photonView;

    void Start()
    {
        instance = this;
        _photonView = GetComponent<PhotonView>();
        // ShowMessageLocally("TEST MESSAGE");
    }

    [PunRPC]
    public void ShowMessageLocally(string text)
    {
        var message = GameObject.Instantiate<GameObject>(chatMessagePrefab, chatWindow);
        message.GetComponent<Text>().text = text;
        
        if (chatWindow.childCount > maxMessages)
        {
            Destroy(chatWindow.GetChild(0).gameObject);
        }
    }

    public void SendMessage(string text)
    {
        var message = text.Trim();
        if (message == "")
        {
            return;
        }
        
        _photonView.RPC("ShowMessageLocally", RpcTarget.All, message);
    }
}
