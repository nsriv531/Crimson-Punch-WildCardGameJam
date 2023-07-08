using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Character : NetworkBehaviour
{
    [Header("Debugging")]
    [ReadOnly, SerializeField] private bool _alive = true;

    [HideInInspector] public bool alive { get => _alive; private set => _alive = value; }

    private NetworkVariable<bool> n_alive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public static Dictionary<ulong, Character> clientIds;


    // Awake -> OnEnable -> OnNetworkSpawn -> Start
    void Awake()
    {
        n_alive.OnValueChanged += (bool previous, bool current) => alive = current;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        if (clientIds == null) clientIds = new Dictionary<ulong, Character>();
        clientIds.Add(OwnerClientId, this);
    }

    public override void OnNetworkDespawn()
    {
        clientIds.Remove(OwnerClientId);
        base.OnNetworkDespawn();
    }

    /// <summary>
    /// Kill this Character.
    /// </summary>
    public void Kill()
    {
        n_alive.Value = false;

        // TODO: animations
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool wasEnabled = GUI.enabled;

        Character script = target as Character;
        GUI.enabled = Application.isPlaying && script.alive;
        if (GUILayout.Button("Kill")) script.Kill();

        GUI.enabled = wasEnabled;
    }
}
#endif
