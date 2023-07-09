using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvents : MonoBehaviour
{
    public UnityEvent onTriggerEnter = new UnityEvent();
    public UnityEvent onTriggerStay = new UnityEvent();
    public UnityEvent onTriggerExit = new UnityEvent();
    public UnityEvent onCollisionEnter = new UnityEvent();
    public UnityEvent onCollisionStay = new UnityEvent();
    public UnityEvent onCollisionExit = new UnityEvent();

    [HideInInspector] public Collision collision;
    [HideInInspector] public Collider other;

    public void OnTriggerEnter(Collider other)
    {
        this.other = other;
        onTriggerEnter.Invoke();
    }

    public void OnTriggerStay(Collider other)
    {
        this.other = other;
        onTriggerStay.Invoke();
    }

    public void OnTriggerExit(Collider other)
    {
        this.other = other;
        onTriggerExit.Invoke();
        this.other = null;
    }

    public void OnCollisionEnter(Collision collision)
    {
        this.collision = collision;
        onCollisionEnter.Invoke();
    }

    public void OnCollisionExit(Collision collision)
    {
        this.collision = collision;
        onCollisionExit.Invoke();
        this.collision = null;
    }

    public void OnCollisionStay(Collision collision)
    {
        this.collision = collision;
        onCollisionStay.Invoke();
    }


}
