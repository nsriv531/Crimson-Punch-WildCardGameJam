using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviourPunCallbacks
{
    public GameObject roomJoinPanel;
    public InputField nicknameInputField;
    public InputField roomNameInputField;

    public GameObject roomPanel;
    public GameObject hostOnlyRoomPanel;
    
    public Text playersLabel;
    public Text playerList;

    public override void OnConnectedToMaster()
    {
        roomJoinPanel.SetActive(true);
    }

    public void JoinOrCreateGame()
    {
        Debug.Log("JOINING!");
        PhotonNetwork.NickName = nicknameInputField.text;
        PhotonNetwork.JoinOrCreateRoom(roomNameInputField.text, new RoomOptions(), TypedLobby.Default);
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
}
