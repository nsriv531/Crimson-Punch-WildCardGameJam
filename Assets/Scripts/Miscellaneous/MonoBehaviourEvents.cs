using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class MonoBehaviourEvents : MonoBehaviour
{
    public UnityEvent start = new UnityEvent();
    public UnityEvent awake = new UnityEvent();
    public UnityEvent onEnable = new UnityEvent();
    public UnityEvent onDisable = new UnityEvent();
    public UnityEvent update = new UnityEvent();
    public UnityEvent fixedUpdate = new UnityEvent();
    public UnityEvent lateUpdate = new UnityEvent();
    public UnityEvent onSetActive = new UnityEvent();

    private bool active;

    void Start() { start.Invoke(); }
    void Awake() { awake.Invoke(); }
    void OnEnable() { onEnable.Invoke(); }
    void OnDisable() { onDisable.Invoke(); active = false; }
    void Update()
    {
        if (!active)
        {
            onSetActive.Invoke();
            active = true;
        }
        update.Invoke();
    }
    void FixedUpdate() { fixedUpdate.Invoke(); }
    void LateUpdate() { lateUpdate.Invoke(); }
}
