using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LowresGraphicsRaycasterClickableModule : LowresGraphicsRaycasterModule
{
    public override void MouseDown(List<RaycastResult> results)
    {
        base.MouseDown(results);
        
        foreach (var result in results)
        {
            var clickable = result.gameObject.GetComponent<IPointerClickHandler>();
            clickable?.OnPointerClick(GeneratePointerEventData());
        }
    }
}
