using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomListEntry : Button
{
    public Color textColorNormal;
    public Color textColorHighlighted;
    
    public Text name;
    public Text status;
    public Text passwordRequired;
    public Text playerCount;
    
    private RoomInfo _roomInfo;

    void Start()
    {
        onClick.AddListener(Click);
    }
    
    public void SetRoomInfo(RoomInfo room)
    {
        name.text = room.Name;
        status.text = ((bool) room.CustomProperties[GameManager.IN_GAME_COLUMN]) ? "IN-GAME" : "LOBBY";
        passwordRequired.text = (room.CustomProperties[GameManager.PASSWORD_COLUMN] as string) != "" ? "YES" : "NO";
        playerCount.text = room.PlayerCount + "/" + room.MaxPlayers;
        
        _roomInfo = room;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        
        name.color = textColorHighlighted;
        status.color = textColorHighlighted;
        passwordRequired.color = textColorHighlighted;
        playerCount.color = textColorHighlighted;
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        
        name.color = textColorNormal;
        status.color = textColorNormal;
        passwordRequired.color = textColorNormal;
        playerCount.color = textColorNormal;
    }

    private void Click()
    {
        RoomJoinMenu.selectedRoom = _roomInfo;
        GameUI.instance.mainSubMenus.Open("Room Join From List");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RoomListEntry))]
public class RoomListEntryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoomListEntry entry = (RoomListEntry) target;
        DrawDefaultInspector();
    }
}
#endif