using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviourPunCallbacks
{
    public Text roomNameLabel;
    public Text playerCountLabel;
    public Text playerListLabel;
    public InputField npcCountField;
    public GameObject hostOnlyRoomPanel;
    
    public override void OnEnable()
    {
        base.OnEnable();
        hostOnlyRoomPanel.SetActive(PhotonNetwork.IsMasterClient);
        roomNameLabel.text = "ROOM NAME: " + PhotonNetwork.CurrentRoom.Name;
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
    
    public void LeaveRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("KickSelf", RpcTarget.OthersBuffered);
        }
        
        PhotonNetwork.LeaveRoom();
        GameUI.instance.mainSubMenus.Open("Server List");
    }
    
    [PunRPC]
    public void KickSelf()
    {
        PhotonNetwork.LeaveRoom();
        GameUI.instance.mainSubMenus.Open("Host Left");
    }
    
    public void StartGame()
    {
        int npcCount = int.Parse(npcCountField.text);
        GameManager.instance.photonView.RPC("BeginMatch", RpcTarget.AllViaServer, npcCount, Random.Range(0, 100));
        photonView.RPC("ReceiveGameStart", RpcTarget.All);
    }

    [PunRPC]
    public void ReceiveGameStart()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.instance.LivingPlayers = new List<Player>(PhotonNetwork.PlayerList);
        GameUI.instance.menuCamera.SetActive(false);
    
        GameUI.instance.topLevelMenus.Open("Game");
        GameUI.instance.inGameSubMenus.Open("Unpaused HUD");
        GameUI.instance.livingDeadHUD.Open("Living HUD");
    
        GameUI.instance.UpdatePlayersRemaining(GameManager.instance.LivingPlayers.Count);
    }
    
    private void UpdatePlayerList()
    {
        playerCountLabel.text = PhotonNetwork.PlayerList.Length + " PLAYERS";
        playerListLabel.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListLabel.text += player.NickName + "\n";
        }
    }
}
