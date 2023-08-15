using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHoverColorChange : Selectable
{
    public Color normalColor = Color.white;
    public Color hoverColor = Color.white;

    private Color originalColor;
    private Graphic element;

    void Start()
    {
        element = GetComponent<Graphic>();
        originalColor = element.color;
    }
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        element.color = originalColor * hoverColor; //Or however you do your color
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        element.color = originalColor * normalColor; //Or however you do your color
    }
}
