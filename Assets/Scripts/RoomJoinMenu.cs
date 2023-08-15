using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomJoinMenu : MonoBehaviourPunCallbacks
{
    public static RoomInfo selectedRoom;
    
    public InputField playerNameField;
    public InputField roomNameField;
    public InputField passwordField;
    public Button joinButton;

    private void Update()
    {
        joinButton.interactable = !string.IsNullOrEmpty(playerNameField.text.Trim()) &&
                                  !(roomNameField && string.IsNullOrEmpty(roomNameField.text.Trim()));
    }
    
    public void JoinRoom()
    {
        var playerName = playerNameField.text;
        var roomName = roomNameField? roomNameField.text : selectedRoom.Name;
        
        PhotonNetwork.NickName = playerName;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        var pass = (string) PhotonNetwork.CurrentRoom.CustomProperties[GameManager.PASSWORD_COLUMN];
        if(pass != "" && passwordField.text != pass)
        {
            PhotonNetwork.LeaveRoom();
            GameUI.ShowDisconnectMessage("FAILED TO JOIN\n\nINCORRECT PASSWORD");
        }
        else if ((bool) PhotonNetwork.CurrentRoom.CustomProperties[GameManager.IN_GAME_COLUMN])
        {
            PhotonNetwork.LeaveRoom();
            GameUI.ShowDisconnectMessage("FAILED TO JOIN\n\nGAME IN PROGRESS");
        }
        else
        {
            var myName = PhotonNetwork.LocalPlayer.NickName.Trim().ToLowerInvariant();
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (!player.IsLocal && player.NickName.Trim().ToLowerInvariant() == myName)
                {
                    PhotonNetwork.LeaveRoom();
                    GameUI.ShowDisconnectMessage("FAILED TO JOIN\n\nPLEASE USE A DIFFERENT NAME");
                    return;
                }
            }
            
            GameUI.instance.mainSubMenus.Open("Room");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        GameUI.ShowDisconnectMessage("FAILED TO JOIN\n\n" + message.ToUpperInvariant());
    }
}
