﻿//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GraphicRaycasterRaycasterExample : MonoBehaviour
{
    public float gameWidth;
    public float gameHeight;
    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    public List<Selectable> hovered = new List<Selectable>();
    public List<Selectable> hoveredLastFrame = new List<Selectable>();

    private List<Selectable> mouseDown = new List<Selectable>();
    
    

    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    void LateUpdate()
    {
        var mousePosCoefficient = new Vector2(gameWidth / Screen.width, gameHeight / Screen.height); 
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

                if (isMouseDown && !mouseDown.Contains(item) && item.isActiveAndEnabled && item.gameObject.activeInHierarchy)
                {
                    mouseDown.Add(item);
                    item.OnPointerDown(pointerEventData);
                    var btn = item.GetComponent<Button>();
                    if (btn != null && btn.isActiveAndEnabled) btn.onClick.Invoke();
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