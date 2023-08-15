using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectProperUICamera : MonoBehaviour
{
    public Canvas canvas;

    private Camera _camera;

    void Update()
    {
        if(!(_camera && _camera.isActiveAndEnabled && _camera.gameObject.activeInHierarchy))
        {
            var mainCamera = Camera.main;
            if(mainCamera)
                _camera = mainCamera.transform.Find("UI Camera")?.GetComponent<Camera>();
            canvas.worldCamera = _camera;
        }
    }
}
