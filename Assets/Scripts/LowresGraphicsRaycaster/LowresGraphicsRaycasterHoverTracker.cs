using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(LowresGraphicsRaycaster))]
public class LowresGraphicsRaycasterHoverTracker<T> : LowresGraphicsRaycasterModule
{
    protected readonly HashSet<T> Hovered = new HashSet<T>();
    
    public override void Tick(List<RaycastResult> results)
    {
        Hovered.Clear();
        foreach (var result in results)
        {
            Hovered.UnionWith(result.gameObject.GetComponents<T>());
        }
    }
}
