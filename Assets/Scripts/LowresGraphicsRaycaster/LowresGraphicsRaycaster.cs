using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LowresGraphicsRaycaster : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(384, 216);
    
    private GraphicRaycaster _raycaster;
    private EventSystem _eventSystem;
    
    private PointerEventData _pointerEventData;
    private readonly List<ILowresGraphicsRaycasterTick> _modules = new List<ILowresGraphicsRaycasterTick>();
    private bool _mouseDownLastFrame = false;

    void Start()
    {
        _raycaster = GetComponent<GraphicRaycaster>();
        _eventSystem = FindObjectOfType<EventSystem>();
    }

    void LateUpdate()
    {
        var isMouseDown = Input.GetMouseButton(0);

        _pointerEventData = new PointerEventData(_eventSystem)
        {
            position = GetLowresMousePosition()
        };

        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(_pointerEventData, results);

        foreach (var module in _modules)
            module.Tick(results);
        
        if (isMouseDown && !_mouseDownLastFrame)
            foreach (var module in _modules)
                module.MouseDown(results);
        else if (!isMouseDown && _mouseDownLastFrame)
            foreach (var module in _modules)
                module.MouseUp(results);
        else if (isMouseDown && _mouseDownLastFrame)
            foreach (var module in _modules)
                module.MouseHeld(results);
        
        _mouseDownLastFrame = isMouseDown;
    }

    public void RegisterModule(ILowresGraphicsRaycasterTick module)
    {
        _modules.Add(module);
    }
    
    public Vector3 GetLowresMousePosition()
    {
        var mousePosCoefficient =
            new Vector2(referenceResolution.x / Screen.width, referenceResolution.y / Screen.height);
        return Input.mousePosition * mousePosCoefficient;
    }
}
