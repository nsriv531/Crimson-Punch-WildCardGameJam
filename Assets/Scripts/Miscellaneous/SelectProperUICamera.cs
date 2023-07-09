using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectProperUICamera : MonoBehaviour
{
    public Canvas canvas;

    private Camera _camera;

    void Update()
    {
        if(!(_camera && _camera.isActiveAndEnabled && _camera.gameObject.activeInHierarchy)) {
            _camera = Camera.main.transform.Find("UI Camera").GetComponent<Camera>();
            canvas.worldCamera = _camera;
        }
    }
}
