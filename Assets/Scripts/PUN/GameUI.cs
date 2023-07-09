using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviourPunCallbacks
{
    public static GameUI instance;
    
    public GameObject menuCamera;
    public GameObject roomJoinPanel;
    public InputField nicknameInputField;
    public InputField roomNameInputField;

    public GameObject roomPanel;
    public Text playersLabel;
    public Text playerList;
    
    public GameObject hostOnlyRoomPanel;
    public InputField npcCountInputField;
    
    public GameObject hud;
    public Text playersRemainingLabel;
    public GameObject femaleIndicator;
    public GameObject maleIndicator;

    private int npcCount = 128;

    void Awake()
    {
        instance = this;
    }

    public override void OnConnectedToMaster()
    {
        roomJoinPanel.SetActive(true);
    }

    public void JoinOrCreateGame()
    {
        PhotonNetwork.NickName = nicknameInputField.text;
        PhotonNetwork.JoinOrCreateRoom(roomNameInputField.text, new RoomOptions(), TypedLobby.Default);
    }

    public void LeaveGame()
    { 
        PhotonNetwork.LeaveRoom();
        roomPanel.SetActive(false);
        roomJoinPanel.SetActive(true);
        hud.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        GameManager.instance.photonView.RPC("BeginMatch", RpcTarget.AllBuffered, npcCount);
        photonView.RPC("ReceiveGameStart", RpcTarget.AllBuffered);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    [PunRPC]
    public void ReceiveGameStart()
    {
        GameManager.instance.livingPlayers = PhotonNetwork.PlayerList.Length;
        menuCamera.SetActive(false);
        roomPanel.SetActive(false);
        hud.SetActive(true);
        UpdatePlayersRemaining(GameManager.instance.livingPlayers);
    }

    public void SubmitPlayerCount()
    {
        if (!int.TryParse(npcCountInputField.text.Trim(), out npcCount))
        {
            npcCount = 128;
            npcCountInputField.text = npcCount.ToString();
        }
    }

    public override void OnJoinedRoom()
    {
        roomJoinPanel.SetActive(false);
        roomPanel.SetActive(true);
        hostOnlyRoomPanel.SetActive(PhotonNetwork.IsMasterClient);
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        playersLabel.text = PhotonNetwork.PlayerList.Length + " PLAYERS";
        playerList.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerList.text += player.NickName + "\n";
        }
    }

    public void IndicateGender(bool isMale)
    {
        (isMale? maleIndicator : femaleIndicator).SetActive(true);
    }
    
    public void UpdatePlayersRemaining(int playersRemaining)
    {
        playersRemainingLabel.text = playersRemaining + " PLAYERS REMAIN";
    }
}
