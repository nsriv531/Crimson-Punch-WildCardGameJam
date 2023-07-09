using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ParticleEffectPlayer : MonoBehaviour
{
    [SerializeField] private bool _deParentOnPlay = true;

    public UnityEvent onPlay = new UnityEvent();

    private List<ParticleSystem> _particleSystems;
    [HideInInspector] public List<ParticleSystem> particleSystems
    {
        get
        {
            if (_particleSystems == null) _particleSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
            return _particleSystems;
        }
    }

    public void Play()
    {
        if (_deParentOnPlay) transform.SetParent(null);
        foreach(ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Play();
        }
        onPlay.Invoke();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(ParticleEffectPlayer))]
public class ExplosionEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool wasEnabled = GUI.enabled;

        ParticleEffectPlayer script = target as ParticleEffectPlayer;
        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Play")) script.Play();

        GUI.enabled = wasEnabled;
    }
}
#endif