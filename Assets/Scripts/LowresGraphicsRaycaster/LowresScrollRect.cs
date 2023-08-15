using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LowresScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    
    public RectTransform viewport;
    public RectTransform content;
    public RectTransform scrollbarContainer;
    
    private RectTransform _rectTransform;
    private float _width;
    
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _width = _rectTransform.sizeDelta.x;
    }

    void Update()
    {
        _rectTransform.sizeDelta = new Vector2(_width, scrollbarContainer.sizeDelta.y * (viewport.sizeDelta.y / content.sizeDelta.y));
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }
}