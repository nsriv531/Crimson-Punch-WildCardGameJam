using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DancerMortality : MonoBehaviourPunCallbacks
{
    public bool isPlayer;

    public UnityEvent onDeath = new UnityEvent();

    public void Kill()
    {
        photonView.RPC("KillRPC", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void KillRPC()
    {
        PhotonNetwork.Destroy(gameObject);
        // TODO: play death effect

        onDeath.Invoke(); // Does this go here?

        if (photonView.IsMine)
        {
            // TODO: move camera to "spectator" view overlooking the map
        }

        if (isPlayer)
        {
            GameManager.instance.PlayerKilled();
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(DancerMortality))]
public class DancerMortalityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool wasEnabled = GUI.enabled;

        DancerMortality script = target as DancerMortality;
        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Kill")) script.Kill();

        GUI.enabled = wasEnabled;
    }
}
#endif