using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerKill : MonoBehaviour
{
    public float range;
    public float cooldown;

    private float _timeRemaining = 0f;
    public GameObject attackEffect;
    
    public UnityEvent onAttack;

    void Update()
    {
        if (_timeRemaining > 0f)
        {
            _timeRemaining -= Time.deltaTime;
            GameUI.instance.cooldownLabel.text = "ATTACK COOLDOWN\n" + _timeRemaining.ToString("0.00");
        }
        else if(Input.GetMouseButtonDown(0) && !PauseMenu.IsOpen)
        {
            Attack();
            _timeRemaining = cooldown;
        }
        else
        {
            GameUI.instance.cooldownLabel.text = "";
        }
        
    }
    
    void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
        {
            if (hit.collider.CompareTag("NPC") || hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<DancerMortality>().Kill(PhotonNetwork.LocalPlayer.NickName);
            }
        }
        StartCoroutine(AttackEffects());
        onAttack.Invoke();
    }

    IEnumerator AttackEffects()
    {
        attackEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackEffect.SetActive(false);
    }
}
