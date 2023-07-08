using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Targeting : MonoBehaviour
{
    /// <summary>
    /// Invoked whenever this Character targets another Character.
    /// </summary>
    [Tooltip("Invoked whenever this Character targets another Character.")]
    public UnityEvent onTargetSet = new UnityEvent();
    /// <summary>
    /// Invoked whenever another Character targets this Character.
    /// </summary>
    [Tooltip("Invoked whenever another Character targets this Character.")]
    public UnityEvent onTargeted = new UnityEvent();

    /// <summary>
    /// Invoked whenever this Character clears their target.
    /// </summary>
    [Tooltip("Invoked whenever this Character clears their target.")]
    public UnityEvent onTargetCleared = new UnityEvent();

    /// <summary>
    /// Invoked whenever another Character who is targeting this Character clears their target.
    /// </summary>
    [Tooltip("Invoked whenever another Character who is targeting this Character clears their target.")]
    public UnityEvent onUntargeted = new UnityEvent();

    
    [ReadOnly, SerializeField] private Character _target;
    [ReadOnly, SerializeField] private Targeting _targetTargeting;
    [ReadOnly] public List<Character> targetOf = new List<Character>();

    [SerializeField] private bool verbose = false;

    [SerializeField] private GameObject debugTargetObject;

    [HideInInspector] public Character target
    {
        get => _target;
        private set => _target = value;
    }

    [HideInInspector] public Targeting targetTargeting
    {
        get => _targetTargeting;
        private set => _targetTargeting = value;
    }

    private Character _character;
    [HideInInspector] public Character character
    {
        get
        {
            if (_character == null) _character = GetComponentInParent<Character>(true);
            if (_character == null) throw new System.Exception("Missing Character component in any parent of " + gameObject);
            return _character;
        }
    }
    
    public void SetTarget(Character target)
    {
        if (verbose) Debug.Log("SetTarget " + target.gameObject, gameObject);

        // If we already have a target, clear our target.
        if (this.target != null) ClearTarget();

        // Set the new target
        this.target = target;

        targetTargeting = target.GetComponentInChildren<Targeting>(true);
        if (targetTargeting == null) throw new System.Exception("Failed to find a Targeting component attached to Character " + target.gameObject);

        targetTargeting.targetOf.Add(character);

        onTargetSet.Invoke();
        targetTargeting.onTargeted.Invoke();
    }
    public void SetTarget(GameObject target) => SetTarget(target.GetComponent<Character>());

    public void ClearTarget()
    {
        if (verbose) Debug.Log("ClearTarget", gameObject);

        if (target == null) throw new System.Exception("Cannot clear target because there is no target set for " + gameObject);
        if (targetTargeting == null) throw new System.Exception("targetTargeting is null for " + gameObject);

        if (!targetTargeting.targetOf.Contains(character)) throw new System.Exception("Failed to find this Character (" + character.gameObject + ") in the targetOf list for " + targetTargeting.gameObject);
        targetTargeting.targetOf.Remove(character);

        target = null;
        targetTargeting.onUntargeted.Invoke();
        targetTargeting = null;
        onTargetCleared.Invoke();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Targeting))]
public class TargetingEditor : Editor
{
    private SerializedProperty onTargetSet, onTargeted, onTargetCleared, onUntargeted, _target, _targetTargeting, targetOf,
        debugTargetObject, verbose;

    void OnEnable()
    {
        onTargetSet = serializedObject.FindProperty(nameof(onTargetSet));
        onTargeted = serializedObject.FindProperty(nameof(onTargeted));
        onTargetCleared = serializedObject.FindProperty(nameof(onTargetCleared));
        onUntargeted = serializedObject.FindProperty(nameof(onUntargeted));
        _target = serializedObject.FindProperty(nameof(_target));
        _targetTargeting = serializedObject.FindProperty(nameof(_targetTargeting));
        targetOf = serializedObject.FindProperty(nameof(targetOf));
        debugTargetObject = serializedObject.FindProperty(nameof(debugTargetObject));
        verbose = serializedObject.FindProperty(nameof(verbose));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.ScriptField();
        serializedObject.Update();

        bool wasEnabled = GUI.enabled;

        Targeting script = target as Targeting;

        EditorGUILayout.PropertyField(onTargetSet);
        EditorGUILayout.PropertyField(onTargeted);
        EditorGUILayout.PropertyField(onTargetCleared);
        EditorGUILayout.PropertyField(onUntargeted);

        EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(verbose);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(_target);
        EditorGUILayout.PropertyField(_targetTargeting);

        EditorGUILayout.BeginHorizontal();
        {
            bool doTarget = GUILayout.Button("Set Target");
            EditorGUILayout.ObjectField(debugTargetObject, GUIContent.none);
            if (doTarget) script.SetTarget(debugTargetObject.objectReferenceValue as GameObject);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Clear Target")) script.ClearTarget();

        EditorGUILayout.LabelField("Target Of");
        EditorGUI.indentLevel++;
        for (int i = 0; i < targetOf.arraySize; i++)
        {
            var item = targetOf.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(item);
        }
        EditorGUI.indentLevel--;

        GUI.enabled = wasEnabled;
        serializedObject.ApplyModifiedProperties();
    }
}
#endif