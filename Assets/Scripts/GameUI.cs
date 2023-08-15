using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class GameUI : MonoBehaviourPunCallbacks
{
    public static GameUI instance;

    public GameObject menuCamera;
    
    public Text playersLabel;
    public Text playerList;
    
    public GameObject hostOnlyRoomPanel;
    public InputField npcCountInputField;
    public Text playersRemainingLabel;
    public Text killedByLabel;
    public Text winnerLabel;
    public Text cooldownLabel;
    public Text roomErrorLabel;
    public Text regionLabel;
    
    [Header("Menus")]
    public ExclusiveMenu topLevelMenus; // In-game vs main menu and co
    public ExclusiveMenu mainSubMenus; // Submenus of the main menu
    public ExclusiveMenu inGameSubMenus; // Submenus of the in-game menu
    public ExclusiveMenu livingDeadHUD; // Living vs dead HUD
    public ExclusiveMenu genderIndicator; // Male/female UI icon

    private int npcCount = 128;
    private bool initialStartup = true;

    void Awake()
    {
        instance = this;
        
        // Find all ExclusiveMenu scripts in the scene and initialize them
        var scripts = new List<ExclusiveMenu>();
        var scene = SceneManager.GetActiveScene();

        var rootObjects = scene.GetRootGameObjects();

        foreach (var go in rootObjects)
            scripts.AddRange(go.GetComponentsInChildren<ExclusiveMenu>(true));

        foreach (var script in scripts)
            script.Init();
    }

    void Start()
    {
        topLevelMenus.Open("Menu");
        mainSubMenus.Open("Connecting");
    }

    public override void OnJoinedLobby()
    {
        if (!initialStartup) return;
        
        initialStartup = false;
        mainSubMenus.Open("Main Menu");
        var regionName = GameManager.REGION_TO_NAME.ContainsKey(PhotonNetwork.CloudRegion);
        regionLabel.text = "CURRENT REGION:\n" + (regionName? GameManager.REGION_TO_NAME[PhotonNetwork.CloudRegion] : PhotonNetwork.CloudRegion);
    }

    public void ReturnToSetup()
    {
        PauseMenu.IsOpen = false;
        topLevelMenus.Open("Menu");
        mainSubMenus.Open("Room");
        
        winnerLabel.gameObject.SetActive(false);
        menuCamera.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void LeaveGame()
    { 
        GameManager.instance.LeaveGame();
        
        PauseMenu.IsOpen = false;
        topLevelMenus.Open("Menu");
        mainSubMenus.Open("Server List");
        menuCamera.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public override void OnJoinedRoom()
    {
        mainSubMenus.Open("Room");
    }

    public void IndicateGender(bool isMale)
    {
        genderIndicator.Open(isMale? "Male Indicator" : "Female Indicator");
    }
    
    public void UpdatePlayersRemaining(int playersRemaining)
    {
        playersRemainingLabel.text = playersRemaining + " PLAYERS REMAIN";
    }

    public void MainMenuQuit()
    {
        Application.Quit();
    }
    
    public static void ShowDisconnectMessage(string message)
    {
        GameUI.instance.mainSubMenus.Open("Room Join Failed");
        GameUI.instance.roomErrorLabel.text = message;
    }

    public void RegionButtonClick(string region)
    {
        initialStartup = true;
        mainSubMenus.Open("Connecting");
        GameManager.instance.ChangeRegion(region);
    }
}
