#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SerializedObjectExtension
{
    /// <summary>
    /// Show the "Script" property field which usually appears at the top of the inspector, in read-only mode.
    /// You can follow this up with DrawPropertiesExcluding(serializedObject, "m_Script") to draw the rest of
    /// the properties excluding the "Script" field.
    /// </summary>
    public static void ScriptField(this SerializedObject serializedObject)
    {
        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        GUI.enabled = true;
    }
}

#endif