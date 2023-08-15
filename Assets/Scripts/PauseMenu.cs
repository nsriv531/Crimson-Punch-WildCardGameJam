using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool IsOpen;

    private void OnEnable()
    {
        IsOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.instance._gameState == GameManager.GameState.Playing)
        {
            IsOpen = !IsOpen;
            if (IsOpen) Open();
            else Close();
        }
    }

    public static void Open()
    {
        IsOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameUI.instance.inGameSubMenus.Open("Pause Menu");
    }
    
    public static void Close()
    {
        IsOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameUI.instance.inGameSubMenus.Open("Unpaused HUD");
    }
}
