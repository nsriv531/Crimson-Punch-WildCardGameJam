using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEscapeListener : MonoBehaviour
{
    [Tooltip("If true, set OnEscapeMessage.success = true after OnEscape is finished being called.")]
    [SerializeField] private bool assertSuccess = false;

    [SerializeField] private UnityEvent onEscape = new UnityEvent();

    public void OnEscape(OnEscapeMessage message)
    {
        //Debug.Log("OnEscape", gameObject);
        onEscape.Invoke();
        message.success = assertSuccess;
    }
}
