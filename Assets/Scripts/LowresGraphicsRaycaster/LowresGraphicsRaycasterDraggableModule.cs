using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LowresGraphicsRaycasterDraggableModule : LowresGraphicsRaycasterModule
{
    private readonly HashSet<IDragHandler> _dragHandlers = new HashSet<IDragHandler>();
    private readonly HashSet<IEndDragHandler> _endHandlers = new HashSet<IEndDragHandler>();
    
    private Vector3 _lastMousePosition;
    
    public override void MouseDown(List<RaycastResult> results)
    {
        base.MouseDown(results);
        
        foreach (var result in results)
        {
            var draggable = result.gameObject.GetComponent<IBeginDragHandler>();
            draggable?.OnBeginDrag(GeneratePointerEventData());
            
            var dragHandler = result.gameObject.GetComponent<IDragHandler>();
            if (dragHandler != null)
            {
                _dragHandlers.Add(dragHandler);
            }
            
            var endHandler = result.gameObject.GetComponent<IEndDragHandler>();
            if (endHandler != null)
            {
                _endHandlers.Add(endHandler);
            }
        }
    }

    public override void Tick(List<RaycastResult> results)
    {
        base.Tick(results);
        
        var mousePosition = Input.mousePosition;
        var delta = mousePosition - _lastMousePosition;
        _lastMousePosition = mousePosition;
        
        foreach(var dragHandler in _dragHandlers)
        {
            var eventData = GeneratePointerEventData();
            eventData.delta = delta;
            eventData.dragging = true;
            dragHandler.OnDrag(eventData);
        }
    }

    public override void MouseUp(List<RaycastResult> results)
    {
        base.MouseUp(results);
        
        foreach(var endHandler in _endHandlers)
        {
            endHandler.OnEndDrag(GeneratePointerEventData());
        }
        
        _dragHandlers.Clear();
        _endHandlers.Clear();
    }
}
