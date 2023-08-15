//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GraphicRaycasterRaycasterExample : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(384, 216);
    public float clickCooldown = 0.1f;
    
    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    public List<Selectable> hovered = new List<Selectable>();
    public List<Selectable> hoveredLastFrame = new List<Selectable>();

    private List<Selectable> mouseDown = new List<Selectable>();
    
    public bool isOnCooldown = false;
    

    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    void LateUpdate()
    {
        var mousePosCoefficient = new Vector2(referenceResolution.x / Screen.width, referenceResolution.y / Screen.height); 
        var isMouseDown = Input.GetMouseButton(0);

        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition * mousePosCoefficient;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        hovered.Clear();
        
        bool clickedSomething = false;
        foreach (RaycastResult result in results)
        {
            var selectables = result.gameObject.GetComponents<Selectable>();
            foreach (var item in selectables)
            {
                if (!hovered.Contains(item)) hovered.Add(item);
                item.OnPointerEnter(pointerEventData);

                if (isMouseDown && !isOnCooldown && !mouseDown.Contains(item) && item.isActiveAndEnabled && item.gameObject.activeInHierarchy && item.IsInteractable())
                {
                    mouseDown.Add(item);
                    item.OnPointerDown(pointerEventData);
                    var btn = item.GetComponent<Button>();
                    // var scrollable = item.GetComponent<Scrollbar>();
                    
                    if (btn != null && btn.isActiveAndEnabled && btn.IsInteractable()) 
                        btn.onClick.Invoke();
                    
                    // if (scrollable != null && scrollable.isActiveAndEnabled && scrollable.IsInteractable())
                    //     scrollable.OnBeginDrag(pointerEventData);
                    
                    
                    
                    clickedSomething = true;
                }
            }
        }
        
        if (clickedSomething)
        {
            isOnCooldown = true;
            // StartCoroutine(RefreshCooldown());
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

            isOnCooldown = false;
        }
    }

    IEnumerator RefreshCooldown()
    {
        yield return new WaitForSeconds(clickCooldown);
        isOnCooldown = false;
    }
}