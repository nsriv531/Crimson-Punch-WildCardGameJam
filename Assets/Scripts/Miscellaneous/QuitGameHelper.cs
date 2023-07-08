using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuitGameHelper : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        if (Application.isEditor) EditorApplication.isPlaying = false;
        else Application.Quit();
#else
        Application.Quit();
#endif
    }
}
