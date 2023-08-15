
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class HostGameMenu : MonoBehaviour
{
    public InputField playerNameInputField;
    public InputField gameNameInputField;
    public InputField passwordInputField;
    public InputField maxPlayersInputField;
    public Button hostGameButton;

    void Update()
    {
        hostGameButton.interactable = !string.IsNullOrEmpty(playerNameInputField.text.Trim()) &&
                                 !string.IsNullOrEmpty(gameNameInputField.text.Trim()) &&
                                 !string.IsNullOrEmpty(maxPlayersInputField.text.Trim());
    }
    
    public void HostGame()
    {
        var playerName = playerNameInputField.text;
        var gameName = gameNameInputField.text;
        var password = passwordInputField.text;
        var maxPlayers = int.Parse(maxPlayersInputField.text);
        
        PhotonNetwork.NickName = playerName;
        PhotonNetwork.CreateRoom(gameName, new RoomOptions()
        {
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = (byte) maxPlayers,
            CustomRoomProperties = new Hashtable()
            {
                { GameManager.DUMMY_COLUMN, "1" },
                { GameManager.PASSWORD_COLUMN, password },
                { GameManager.IN_GAME_COLUMN, false }
            },
            CustomRoomPropertiesForLobby = new string[]
            {
                GameManager.DUMMY_COLUMN, 
                GameManager.PASSWORD_COLUMN, 
                GameManager.IN_GAME_COLUMN
            }
        }, GameManager.defaultLobby);
    }
}
