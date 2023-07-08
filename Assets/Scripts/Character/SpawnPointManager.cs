using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Attaches to a child of a Character component and can be used to handle spawning the Character at a SpawnPoint.
/// </summary>
public class SpawnPointManager : MonoBehaviour
{
    private Character _character;
    [HideInInspector] public Character character
    {
        get
        {
            if (_character == null) _character = GetComponentInParent<Character>(true);
            if (_character == null) throw new System.Exception("Failed to find a Character component in any parent of " + gameObject);
            return _character;
        }
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        List<SpawnPoint> spawnPoints = new List<SpawnPoint>(FindObjectsOfType<SpawnPoint>());
        if (spawnPoints.Count == 0) throw new System.Exception("No SpawnPoints found!");
        return spawnPoints.Random();
    }

    public void SpawnCopyAtRandomSpawnPoint()
    {
        SpawnCopyAtSpawnPoint(GetRandomSpawnPoint());
    }

    public void SpawnCopyAtSpawnPoint(SpawnPoint spawnPoint)
    {
        spawnPoint.SpawnCopy(character);
    }

    public void SpawnAtRandomSpawnPoint()
    {
        SpawnAtSpawnPoint(GetRandomSpawnPoint());
    }

    public void SpawnAtSpawnPoint(SpawnPoint spawnPoint)
    {
        spawnPoint.Spawn(character);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SpawnPointManager))]
public class SpawnPointManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool wasEnabled = GUI.enabled;

        //EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);
        
        SpawnPointManager script = target as SpawnPointManager;

        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Spawn at Random Spawn Point")) script.SpawnAtRandomSpawnPoint();
        if (GUILayout.Button("Spawn Copy at Random Spawn Point")) script.SpawnCopyAtRandomSpawnPoint();

        GUI.enabled = wasEnabled;
    }
}
#endif