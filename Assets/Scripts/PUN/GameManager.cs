using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    
    public GameObject player;
    public GameObject maleNPC;
    public GameObject femaleNPC;
    
    public Transform spawnpoint;
    
    public int livingPlayers;

    public GameObject localPlayerInstance;
    private List<Player> players = new List<Player>();
    
    void Start()
    {
        GameManager.instance = this;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
    
    public override void OnJoinedRoom()
    {
        players.Add(PhotonNetwork.LocalPlayer);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        players.Add(newPlayer);
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        players.Remove(otherPlayer);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.Space))
        {
            BeginMatch();
        }
    }

    public void BeginMatch()
    {
        livingPlayers = players.Count;
        
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SpawnMyPlayer", RpcTarget.All);
            
            {
                var instance = PhotonNetwork.Instantiate(femaleNPC.name, spawnpoint.position, spawnpoint.rotation);
            
            }
        }
    }

    [PunRPC]
    public void SpawnMyPlayer()
    {
        var pt = Random.insideUnitCircle;
        var instance = PhotonNetwork.Instantiate(player.name, spawnpoint.position + new Vector3(pt.x, 0.0f, pt.y) * 5.0f, spawnpoint.rotation);
    }

    public void PlayerKilled()
    {   
        livingPlayers--;
        if (livingPlayers <= 1)
        {
            // TODO: win condition
        }
    }
}
