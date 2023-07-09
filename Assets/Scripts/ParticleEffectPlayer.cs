using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ParticleEffectPlayer : MonoBehaviour
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
        transform.SetParent(null);
        foreach(ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Play();
        }
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