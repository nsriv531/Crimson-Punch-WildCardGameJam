using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public UnityEvent onWinCondition = new UnityEvent();
    
    enum GameState
    {
        Lobby,
        Playing
    }
    
    private GameState _gameState = GameState.Lobby;
    
    [Header("References")]
    public GameObject player;
    public GameObject maleNPC;
    public GameObject femaleNPC;

    public int livingPlayers;

    public GameObject localPlayerInstance;

    public Transform mapTopLeft;
    public Transform mapBottomRight;
    
    private List<GameObject> npcs = new List<GameObject>();
    
    void Start()
    {
        GameManager.instance = this;
        PhotonNetwork.ConnectUsingSettings();
        Screen.SetResolution(792, 432, false, 60);
    }

    public override void OnConnectedToMaster()
    {
        
    }
    
    public override void OnJoinedRoom()
    {
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
    }
    
    [PunRPC]
    public void BeginMatch(int npcCount = 128)
    {
        _gameState = GameState.Playing;
        livingPlayers = PhotonNetwork.PlayerList.Length;
        
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SpawnMyPlayer", RpcTarget.All);

            for (int i = 0; i < npcCount; i++)
            {
                SpawnNPC();
            }
        }
    }

    public void EndMatch()
    {
        _gameState = GameState.Lobby;
        livingPlayers = 0;
        foreach (var npc in npcs)
        {
            PhotonNetwork.Destroy(npc);
        }
        npcs.Clear();
        photonView.RPC("DeleteMyPlayer", RpcTarget.All);
    }
    
    [PunRPC]
    private void DeleteMyPlayer()
    {
        if (localPlayerInstance != null)
        {
            PhotonNetwork.Destroy(localPlayerInstance);
            localPlayerInstance = null;
        }
    }

    Vector3 GetRandomPointOnNavMesh()
    {
        while (true) {
            var topLeft = mapTopLeft.position;
            var bottomRight = mapBottomRight.position;
            var randomPt = new Vector3(Random.Range(topLeft.x, bottomRight.x), Random.Range(0.0f, 10.0f), Random.Range(topLeft.z, bottomRight.z));
            var navMeshPt = NavMesh.SamplePosition(randomPt, out var hit, 100.0f, NavMesh.AllAreas);
            if (navMeshPt) return hit.position;
        }
    }

    // Spawn the NPC in a random point within the top left and bottom right of the map
    void SpawnNPC()
    {
        var spawnPoint = GetRandomPointOnNavMesh();
        var variant = Random.Range(0.0f, 1.0f) < 0.5f ? maleNPC : femaleNPC;
        var instance = PhotonNetwork.Instantiate(variant.name, spawnPoint, Quaternion.identity);
        npcs.Add(instance);
    }

    [PunRPC]
    public void SpawnMyPlayer()
    {
        var spawnPoint = GetRandomPointOnNavMesh();
        localPlayerInstance = PhotonNetwork.Instantiate(player.name, spawnPoint, Quaternion.identity);
    }

    public void PlayerKilled()
    {   
        livingPlayers--;
        GameUI.instance.UpdatePlayersRemaining(livingPlayers);
        if (livingPlayers <= 1)
        {
            // TODO: win condition
            OnWinCondition();
        }
    }



    public void OnWinCondition()
    {
        foreach (ConfettiBlaster blaster in FindObjectsOfType<ConfettiBlaster>())
        {
            blaster.GetComponent<ParticleEffectPlayer>().Play();
        }

        onWinCondition.Invoke();
    }
}




#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool wasEnabled = GUI.enabled;

        GUI.enabled = Application.isPlaying;

        GameManager script = target as GameManager;
        if (GUILayout.Button("Test OnWinCondition")) script.OnWinCondition();

        GUI.enabled = wasEnabled;
    }
}
#endif