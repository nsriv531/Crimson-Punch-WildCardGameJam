//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GraphicRaycasterRaycasterExample : MonoBehaviour
{
    public float mousePosCoefficient = 0.5f;
    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    public List<Selectable> hovered = new List<Selectable>();
    public List<Selectable> hoveredLastFrame = new List<Selectable>();

    List<Selectable> mouseDown = new List<Selectable>();

    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    void LateUpdate()
    {
        var isMouseDown = Input.GetMouseButton(0);

        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition * mousePosCoefficient;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        hovered.Clear();
        foreach (RaycastResult result in results)
        {
            var selectables = result.gameObject.GetComponents<Selectable>();
            foreach (var item in selectables)
            {
                if (!hovered.Contains(item)) hovered.Add(item);
                item.OnPointerEnter(pointerEventData);

                if (isMouseDown && !mouseDown.Contains(item))
                {
                    mouseDown.Add(item);
                    item.OnPointerDown(pointerEventData);
                    if (item.GetComponent<Button>() != null) item.GetComponent<Button>().onClick.Invoke();
                }
            }
        }

        foreach (var item in hoveredLastFrame)
        {
            if (!hovered.Contains(item)) item.OnPointerExit(pointerEventData);
        }

        hoveredLastFrame.Clear();
        hoveredLastFrame.AddRange(hovered);
        
        if(!isMouseDown)
        {
            for (int i = mouseDown.Count - 1; i > -1; i--)
            {
                mouseDown[i].OnPointerUp(pointerEventData);
                mouseDown.RemoveAt(i);
            }
        }
    }
}