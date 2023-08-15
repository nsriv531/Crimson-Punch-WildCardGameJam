using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
public class GameManager : MonoBehaviourPunCallbacks
{
    public static readonly Dictionary<string, string> REGION_TO_NAME = new Dictionary<string, string>()
    {
        { "usw", "NORTH AMERICA" },
        { "sa",  "SOUTH AMERICA" },
        { "eu",  "EUROPE" },
        { "za",  "SOUTH AFRICA" },
        { "in",  "INDIA" },
        { "asia", "ASIA" },
        { "au",  "AUSTRALIA" }
    };
    
    public const string APP_ID_REALTIME = "4ce96075-9d85-4dce-99b2-fdb72687f641";
    
    public static TypedLobby defaultLobby = new TypedLobby("LobbyZero", LobbyType.SqlLobby);

    public const string DUMMY_COLUMN = "C0";
    public const string PASSWORD_COLUMN = "C1";
    public const string IN_GAME_COLUMN = "C2";

    public const int MAX_NPCS = 512;
    
    public static GameManager instance;
    
    private static readonly List<IGameEventListener> Listeners = new List<IGameEventListener>();
    public bool isDead = false;

    public string regionToJoin = null;
    
    public static void RegisterEventListener(IGameEventListener listener)
    {
        Listeners.Add(listener);
    }
    
    public static void UnregisterEventListener(IGameEventListener listener)
    {
        Listeners.Remove(listener);
    }
    
    private static void RaiseEvent(string ev, object msg)
    {
        for(int i = Listeners.Count - 1; i >= 0; i--)
        {
            Listeners[i].OnEventRaised(ev, msg);
        }
    }
    
    public enum GameState
    {
        Lobby,
        Playing
    }
    
    public GameState _gameState = GameState.Lobby;
    
    [Header("References")]
    public GameObject player;
    public GameObject maleNPC;
    public GameObject femaleNPC;

    public GameObject localPlayerInstance;

    public Transform mapTopLeft;
    public Transform mapBottomRight;
    
    public List<GameObject> toCleanUp = new List<GameObject>();
    
    public List<Player> LivingPlayers = new List<Player>();
    private List<GameObject> npcs = new List<GameObject>();

    void Start()
    {
        GameManager.instance = this;
        
        var settings = new AppSettings
        {
            AppIdRealtime = APP_ID_REALTIME
        };
        
        PhotonNetwork.ConnectUsingSettings(settings);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(defaultLobby);
    }
    
    public override void OnJoinedRoom()
    {
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ChatWindow.instance.ShowMessageLocally(otherPlayer.NickName + " LEFT THE GAME");
        PlayerKilled(otherPlayer);
    }

    public void LeaveGame()
    {
        if (isDead)
        {
            localPlayerInstance.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
        }
        
        PhotonNetwork.LeaveRoom();
        EndMatchClientside();
    }
    
    [PunRPC]
    public void BeginMatch(int npcCount = 128, int aiSeed = 0)
    {
        _gameState = GameState.Playing;
        LivingPlayers = new List<Player>(PhotonNetwork.PlayerList);
        DeterministicNPC.InitialiseGlobal(aiSeed);
        GameManager.instance.isDead = false;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
                { IN_GAME_COLUMN, true } 
            });
            photonView.RPC("SpawnMyPlayer", RpcTarget.All);

            for (int i = 0; i < Mathf.Min(npcCount, MAX_NPCS); i++)
            {
                SpawnNPC();
            }
        }
    }

    public void EndMatchClientside()
    {
        RaiseEvent("GameEnd", null);

        // PhotonNetwork.Destroy(localPlayerInstance);
        localPlayerInstance = null;
        
        foreach (GameObject child in toCleanUp)
        {
            Destroy(child);
        }
        toCleanUp.Clear();

        isDead = false;
    }
    
    [PunRPC]
    public void EndMatch()
    {
        _gameState = GameState.Lobby;
        LivingPlayers.Clear();
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
            { IN_GAME_COLUMN, false } 
        });
        
        RaiseEvent("GameEnd", null);

        // PhotonNetwork.Destroy(localPlayerInstance);
        localPlayerInstance = null;
        
        foreach (GameObject child in toCleanUp)
        {
            Destroy(child);
        }
        toCleanUp.Clear();
        
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var p in PhotonNetwork.PlayerList)
            {
                PhotonNetwork.OpCleanActorRpcBuffer(p.ActorNumber);
            }
            
            PhotonNetwork.DestroyAll();
            // foreach (var npc in npcs)
            // {
            //     PhotonNetwork.Destroy(npc);
            // }
            npcs.Clear();
        }
        
        isDead = false;
        GameUI.instance.ReturnToSetup();
    }
    
    public Vector3 GetRandomPointOnNavMesh()
    {
        while (true) {
            var topLeft = mapTopLeft.position;
            var bottomRight = mapBottomRight.position;
            var randomPt = new Vector3(Random.Range(topLeft.x, bottomRight.x), Random.Range(0.0f, 10.0f), Random.Range(topLeft.z, bottomRight.z));
            var navMeshPt = NavMesh.SamplePosition(randomPt, out var hit, 100.0f, NavMesh.AllAreas);
            if (navMeshPt) return hit.position;
        }
    }

    public Vector3 GetRandomPointOnNavMesh(System.Random rng)
    {
        while (true)
        {
            var topLeft = mapTopLeft.position;
            var bottomRight = mapBottomRight.position;
            var x = topLeft.x + rng.NextDouble() * (bottomRight.x - topLeft.x);
            var y = rng.NextDouble() * 10.0f;
            var z = topLeft.z + rng.NextDouble() * (bottomRight.z - topLeft.z);
            var randomPt = new Vector3((float) x, (float) y, (float) z);
            var navMeshPt = NavMesh.SamplePosition(randomPt, out var hit, 100.0f, NavMesh.AllAreas);
            if (navMeshPt) return hit.position;
        }
    }

    // Spawn the NPC in a random point within the top left and bottom right of the map
    void SpawnNPC()
    {
        var spawnPoint = GetRandomPointOnNavMesh();
        var variant = Random.Range(0.0f, 1.0f) < 0.5f ? maleNPC : femaleNPC;
        var instance = PhotonNetwork.InstantiateRoomObject(variant.name, spawnPoint, Quaternion.identity);
        npcs.Add(instance);
    }

    [PunRPC]
    public void SpawnMyPlayer()
    {
        var spawnPoint = GetRandomPointOnNavMesh();
        localPlayerInstance = PhotonNetwork.Instantiate(player.name, spawnPoint, Quaternion.identity);
    }

    public void PlayerKilled(Player player)
    {
        LivingPlayers.Remove(player);
        GameUI.instance.UpdatePlayersRemaining(LivingPlayers.Count);
        if (LivingPlayers.Count <= 1)
        {
            OnPlayerWon();
        }
    }

    private void OnPlayerWon()
    {
        GameUI.instance.winnerLabel.gameObject.SetActive(true);
        if (LivingPlayers.Count == 0)
        {
            GameUI.instance.winnerLabel.text = "NO WINNERS - EVERYONE DIED";
        }
        else if (LivingPlayers.Count == 1)
        {
            GameUI.instance.winnerLabel.text = GetLastPlayer().NickName + " WINS!";
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(WaitAndEndMatch());
        }
    }
    
    IEnumerator WaitAndEndMatch()
    {
        yield return new WaitForSeconds(15);
        photonView.RPC("EndMatch", RpcTarget.All);
    }

    private Player GetLastPlayer()
    {
        return LivingPlayers[0];
    }

    public void ChangeRegion(string region)
    {
        Debug.Log("JOINING REGION:" + region);
        instance.regionToJoin = region;
        PhotonNetwork.Disconnect();
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("DISCONNECTED, JOINING REGION: " + (regionToJoin ?? "null"));
        if(regionToJoin != null)
        {
            Debug.Log("Changing region");
            
            var settings = new AppSettings
            {
                AppIdRealtime = APP_ID_REALTIME,
                FixedRegion = regionToJoin,
            };
        
            PhotonNetwork.ConnectUsingSettings(settings);
            
            regionToJoin = null;
        }
    }
    
}