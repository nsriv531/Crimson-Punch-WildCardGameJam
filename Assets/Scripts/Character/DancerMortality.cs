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
    public GameObject corpsePrefab;

    public UnityEvent onDeath = new UnityEvent();

    public void Kill()
    {
        photonView.RPC("KillRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void KillRPC()
    {
        if(photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
        
        // Raycast downwards and instantiate a corpse there
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f))
        {
            // Instantiate the corpse at the hit's point, with up being the hit's normal
            GameObject corpse = PhotonNetwork.Instantiate(corpsePrefab.name, hit.point + hit.normal * 0.1f, Quaternion.LookRotation(hit.normal));
            corpse.GetComponent<PUNCorpse>().photonView.RPC("SetOrientation", RpcTarget.AllBuffered, hit.normal);
        }


        // PhotonNetwork.Instantiate(corpsePrefab)

        onDeath.Invoke(); // Does this go here?


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