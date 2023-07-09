using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// This script rotates its parent object over time at a constant angular velocity.
[ExecuteInEditMode]
public class Rotate : MonoBehaviour
{
    [Tooltip("When set to true, the Angular Velocity becomes the scale by which to multiply the noise on each axis.")]
    [SerializeField] private bool random = false;
    public Vector3 angularVelocity = Vector3.zero;

    private Quaternion originalRotation;

    // FixedUpdate is called during the main physics loop in Unity
    void Update()
    {
        Vector3 rotation = angularVelocity * Time.deltaTime;
        if (random)
        {
            int ID = gameObject.GetInstanceID();
            rotation = new Vector3(
                angularVelocity.x * (0.5f - Mathf.PerlinNoise(Time.time, ID)),
                angularVelocity.y * (0.5f - Mathf.PerlinNoise(Time.time, ID + 1)),
                angularVelocity.z * (0.5f - Mathf.PerlinNoise(Time.time, ID + 2))
            );
        }
        transform.Rotate(rotation, Space.Self);
    }

    void OnEnable()
    {
        originalRotation = transform.localRotation;
    }

    void OnDisable()
    {
        // Reset the rotation to its original
        transform.localRotation = originalRotation;
    }

    void OnDrawGizmos()
    {
        // Your gizmo drawing thing goes here if required...

#if UNITY_EDITOR
        // Ensure continuous Update calls.
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }

}
/*
[CustomEditor(typeof(Rotate))]
public class RotateEditor : Editor
{
    void OnSceneGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            SceneView.RepaintAll();
        }
    }

}
*/