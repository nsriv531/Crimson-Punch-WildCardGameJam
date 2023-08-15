using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomListMenu : MonoBehaviourPunCallbacks
{
    public int roomsPerPage = 10;
    public GameObject roomListContainer;
    public GameObject roomEntryPrefab;
    public Text statusText;
    public Text pageText;
    public InputField searchField;
    
    private List<RoomInfo> _rooms;
    private List<RoomInfo> _roomsCached;
    private int _currentPage = 0;
    
    private void OnEnable()
    {
        base.OnEnable();
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in roomListContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        statusText.text = "LOADING...";
        
        if(!PhotonNetwork.InRoom)
            PhotonNetwork.GetCustomRoomList(GameManager.defaultLobby, GameManager.DUMMY_COLUMN + " = '1'");
    }

    public override void OnJoinedLobby()
    {
        Refresh();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomsCached = roomList;
        _rooms = roomList;
        _currentPage = 0;

        if(_rooms.Count == 0)
        {
            statusText.text = "NO SERVERS FOUND";
        }
        else
        {
            statusText.text = "";
            UpdateSearch();
            ShowPage(0);
        }
    }

    public void ClearServerList()
    {
        foreach (Transform child in roomListContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void ShowPage(int page)
    {
        ClearServerList();
        pageText.text = "PAGE " + (page + 1) + "/" + GetMaxPages();
        for(int i = roomsPerPage * page; i < Mathf.Min(roomsPerPage * (page + 1), _rooms.Count); i++)
        {
            var room = _rooms[i];
            var roomListEntry = Instantiate(roomEntryPrefab, roomListContainer.transform);
            roomListEntry.transform.parent = roomListContainer.transform;
            roomListEntry.GetComponent<RoomListEntry>().SetRoomInfo(room);
        }
    }

    public int GetMaxPages()
    {
        return Mathf.CeilToInt((float) _rooms.Count / roomsPerPage);
    }

    public void PreviousPage()
    {
        if(_currentPage > 0)
            ShowPage(--_currentPage);
    }
    
    public void NextPage()
    {
        if(_currentPage < GetMaxPages() - 1)
            ShowPage(++_currentPage);
    }

    public void UpdateSearch()
    {
        _rooms = new List<RoomInfo>();
        foreach (var room in _roomsCached)
        {
            if (room.Name.ToLower().Contains(searchField.text.ToLower()))
            {
                _rooms.Add(room);
            }
        }
        
        _currentPage = 0;
        ShowPage(0);
    }
}
