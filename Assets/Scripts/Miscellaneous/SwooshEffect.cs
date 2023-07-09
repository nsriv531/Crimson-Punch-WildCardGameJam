using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SwooshEffect : MonoBehaviour
{
    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private Vector3 _endPosition;
    [SerializeField] private Quaternion _startRotation;
    [SerializeField] private Quaternion _endRotation;
    [SerializeField, Min(0f)] private float duration = 1f;

    [Header("Debugging")]
    [ReadOnly, SerializeField] private bool _playing;
    [ReadOnly, SerializeField] private float _timer;

    // Update is called once per frame
    void Update()
    {
        if (_playing)
        {
            _timer = Mathf.Min(_timer + Time.deltaTime, duration);
            if (_timer == duration) OnFinished();
            else DoEffect();
        }
    }

    private void OnFinished()
    {
        _timer = 0f;
        _playing = false;
        gameObject.SetActive(false);
        transform.SetPositionAndRotation(_startPosition, _startRotation);
    }

    public void Play()
    {
        _playing = true;
        gameObject.SetActive(true);
    }

    private void DoEffect()
    {
        float progress = _timer / duration;
        Quaternion newRotation = Quaternion.Lerp(_startRotation, _endRotation, progress);
        Vector3 newPosition = Vector3.Lerp(_startPosition, _endPosition, progress);
        transform.SetPositionAndRotation(newPosition, newRotation);
    }

    public void SetStartPosition()
    {
        _startPosition = transform.position;
    }

    public void SetEndPosition()
    {
        _endPosition = transform.position;
    }

    public void SetStartRotation()
    {
        _startRotation = transform.rotation;
    }

    public void SetEndRotation()
    {
        _endRotation = transform.rotation;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SwooshEffect))]
public class SwooshEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SwooshEffect script = target as SwooshEffect;
        if (GUILayout.Button("Test")) script.Play();
        if (GUILayout.Button("Set Start Position")) script.SetStartPosition();
        if (GUILayout.Button("Set End Position")) script.SetEndPosition();
        if (GUILayout.Button("Set Start Rotation")) script.SetStartRotation();
        if (GUILayout.Button("Set End Rotation")) script.SetEndRotation();
    }
}
#endif