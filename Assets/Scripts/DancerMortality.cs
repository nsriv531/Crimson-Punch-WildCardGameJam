using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DancerMortality : MonoBehaviourPunCallbacks
{
    public bool isPlayer;

    public PlayermodelInfo playermodelInfo;
    public AudioSource deathAudioSource;
    public Text localDeathText;
    
    public GameObject bloodSpatterPrefab;

    public UnityEvent onDeath = new UnityEvent();
    [FormerlySerializedAs("onDeathPlayer")] public UnityEvent onLocalPlayerDeath = new UnityEvent();

    public void Kill(string killerName)
    {
        photonView.RPC("KillRPC", RpcTarget.All, killerName);
    }

    [PunRPC]
    private void KillRPC(string killerName)
    {
        if (isPlayer && photonView.IsMine)
        {
            onLocalPlayerDeath.Invoke();
            GameUI.instance.livingDeadHUD.Open("Dead HUD");
            GameUI.instance.killedByLabel.text = "YOU WERE KILLED BY: " + killerName;
            GameUI.instance.killedByLabel.gameObject.SetActive(true);
            GameManager.instance.isDead = true;
        }
        
        onDeath.Invoke();
        
        if (isPlayer)
        {
            ChatWindow.instance.ShowMessageLocally(photonView.Owner.NickName + " DIED");
            GameManager.instance.PlayerKilled(photonView.Owner);
        }
    }

    public void UnparentCamera()
    {
        Camera.main.transform.parent = null;
    }

    public void PlayDeathSound()
    {
        deathAudioSource.PlayOneShot(playermodelInfo.deathSound);
    }

    public void ShowLocalPlayermodel()
    {
        playermodelInfo.gameObject.SetActive(true);
    }

    public void PlayDeathAnimation()
    {
        // Raycast downwards and set the up direction to the hit's normal
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f))
        {
            transform.up = hit.normal;
            transform.Rotate(transform.up, Random.Range(0f, 360f));
        }
        
        playermodelInfo.PlayDeathAnimation();
    }

    public void EnableBloodParticles()
    {
        playermodelInfo.PlayBloodParticleSystem();
    }

    public void SprayBlood()
    {
        // Raycast 5 times in a random 45 degree cone pointing downwards and instantiate a blood splatter there
        for (int i = 0; i < 5; i++)
        {
            Vector3 direction = Quaternion.Euler(Random.Range(-45f, 45f), Random.Range(0f, 360f), 0f) * Vector3.down;
            if (Physics.Raycast(transform.position + Vector3.up, direction, out RaycastHit hit, 100f))
            {
                // Instantiate the blood splatter at the hit's point, with up being the hit's normal
                GameObject bloodSplatter = Instantiate(bloodSpatterPrefab, hit.point + hit.normal * 0.1f, Quaternion.LookRotation(hit.normal));
                bloodSplatter.transform.forward = -hit.normal;
                GameManager.instance.toCleanUp.Add(bloodSplatter);
                // bloodSplatter.transform.Rotate(hit.normal, Random.Range(0f, 360f));
            }
        }
    }

    public void DestroyUnusedComponents()
    {
        Destroy(GetComponent<Collider>());
        
        if (isPlayer)
        {
            if (photonView.IsMine)
            {
                Destroy(GetComponent<Rigidbody>());
                Destroy(GetComponent<PlayerMovement>());
                Destroy(GetComponent<CameraMovement>());
                Destroy(GetComponent<PlayerKill>());
            }
            else
            {
                Destroy(GetComponent<PlayerSync>());
            }
        }
        else
        {
            Destroy(GetComponent<DeterministicNPC>());
        }
        
        Destroy(GetComponent<DancerMortality>());
    }
}