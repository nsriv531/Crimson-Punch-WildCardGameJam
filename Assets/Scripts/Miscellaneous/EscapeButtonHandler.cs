using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Objects placed in the components list (or one of their children) must contain a method called 'OnEscape' that takes " +
// a single input of type OnEscapeMessage. The OnEscape message must set the attribute 'success' of the OnEscapeMessage input object as 
// true if the Escape button behavior has been handled and false otherwise. When the Escape button is pressed, the components' OnEscape
// methods are called in the order that they appear in the components list.

public class EscapeButtonHandler : MonoBehaviour
{
    [SerializeField] private Mode mode = Mode.BroadcastMessage;

    public List<Component> components = new List<Component>();


    public enum Mode
    {
        BroadcastMessage,
        SendMessage,
        SendMessageUpwards,
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            OnEscape();
        }
    }

    public void OnEscape()
    {
        if (!enabled) return;
        
        //if (character != null)
        //    if (!character.IsOwner) return;

        //Debug.Log(components.GetString());
        foreach (Component component in new List<Component>(components))
        {
            if (component != null)
            {
                if (component.gameObject.activeInHierarchy)
                {
                    OnEscapeMessage message = new OnEscapeMessage(this);
                    if (mode == Mode.BroadcastMessage) component.BroadcastMessage("OnEscape", message, SendMessageOptions.DontRequireReceiver);
                    else if (mode == Mode.SendMessage) component.SendMessage("OnEscape", message, SendMessageOptions.DontRequireReceiver);
                    else if (mode == Mode.SendMessageUpwards) component.SendMessageUpwards("OnEscape", message, SendMessageOptions.DontRequireReceiver);
                    //Debug.Log(component+"   "+message.success);
                    if (message.success) return;
                }
            }
        }
    }
}



public class OnEscapeMessage
{
    public Component component;
    public bool success;

    public OnEscapeMessage(Component component)
    {
        this.component = component;
        success = false;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(EscapeButtonHandler))]
public class EscapeButtonHandlerEditor : Editor
{
    private SerializedProperty components, mode;

    private void OnEnable()
    {
        components = serializedObject.FindProperty(nameof(components));
        mode = serializedObject.FindProperty(nameof(mode));
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("This script uses BroadcastMessage to send the OnEscape event to each component listed below." +
            " The OnEscape method must accept an OnEscapeMessage object, contained inside this component.", MessageType.Info);

        serializedObject.Update();
        serializedObject.ScriptField();

        EditorGUILayout.PropertyField(mode);
        EditorGUILayout.PropertyField(components);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif