using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(LowresGraphicsRaycaster))]
public class LowresGraphicsRaycasterModule : MonoBehaviour, ILowresGraphicsRaycasterTick
{
    protected LowresGraphicsRaycaster Raycaster;
    void Start()
    {
        Raycaster = GetComponent<LowresGraphicsRaycaster>();
        Raycaster.RegisterModule(this);
        Init();
    }
    
    protected virtual void Init() { }
    
    public virtual void Tick(List<RaycastResult> results) { }
    public virtual void MouseHeld(List<RaycastResult> results) { }
    public virtual void MouseDown(List<RaycastResult> results) { }
    public virtual void MouseUp(List<RaycastResult> results) { }

    protected virtual PointerEventData GeneratePointerEventData()
    {
        return new PointerEventData(EventSystem.current)
        {
            position = Raycaster.GetLowresMousePosition()
        };
    }
}

public interface ILowresGraphicsRaycasterTick
{
    void MouseDown(List<RaycastResult> results);
    void MouseUp(List<RaycastResult> results);
    void MouseHeld(List<RaycastResult> results);
    void Tick(List<RaycastResult> results);
}
