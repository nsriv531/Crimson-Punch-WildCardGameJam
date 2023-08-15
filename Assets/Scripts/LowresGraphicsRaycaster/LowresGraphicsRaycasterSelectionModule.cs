using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LowresGraphicsRaycasterSelectionModule : LowresGraphicsRaycasterHoverTracker<Selectable>
{
    private bool _cooldown = false;
    
    private readonly HashSet<Selectable> _hoveredLastFrame = new HashSet<Selectable>();

    public override void Tick(List<RaycastResult> results)
    {
        base.Tick(results);

        var newlyHovered = new HashSet<Selectable>(Hovered);
        newlyHovered.ExceptWith(_hoveredLastFrame);
        foreach (var selectable in newlyHovered)
        {
            selectable.OnPointerEnter(new PointerEventData(EventSystem.current));
        }
        
        var newlyUnhovered = new HashSet<Selectable>(_hoveredLastFrame);
        newlyUnhovered.ExceptWith(Hovered);
        foreach (var selectable in newlyUnhovered)
        {
            selectable.OnPointerExit(new PointerEventData(EventSystem.current));
        }
        
        _hoveredLastFrame.Clear();
        _hoveredLastFrame.UnionWith(Hovered);
    }

    public override void MouseDown(List<RaycastResult> results)
    {
        base.MouseDown(results);
        
        if (_cooldown) return;
        
        foreach (var result in results)
        {
            var selectable = result.gameObject.GetComponent<Selectable>();
            if (!selectable) continue;
            selectable.Select();
            selectable.OnPointerDown(new PointerEventData(EventSystem.current));
            _cooldown = true;
        }
    }
    
    public override void MouseUp(List<RaycastResult> results)
    {
        base.MouseUp(results);
        _cooldown = false;
    }
}
