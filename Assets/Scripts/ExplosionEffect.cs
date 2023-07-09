using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ExplosionEffect : MonoBehaviour
{
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
        foreach(ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Play();
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(ExplosionEffect))]
public class ExplosionEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool wasEnabled = GUI.enabled;

        ExplosionEffect script = target as ExplosionEffect;
        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Play")) script.Play();

        GUI.enabled = wasEnabled;
    }
}
#endif