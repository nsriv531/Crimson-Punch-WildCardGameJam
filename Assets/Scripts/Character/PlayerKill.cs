using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKill : MonoBehaviour
{
    public float range;
    public float cooldown;

    private bool _canAttack = true;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack)
        {
            _canAttack = false;
            StartCoroutine(Attack());
        }
    }
    
    IEnumerator Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
        {
            if (hit.collider.CompareTag("Dancer"))
            {
                hit.collider.GetComponent<Character>().Kill();
            }
        }
        yield return new WaitForSeconds(cooldown);
        _canAttack = true;
    }
}
