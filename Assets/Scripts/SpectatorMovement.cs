using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    public float lerpDelta;
    public Vector3 speed;

    private Vector3 _position;

    void Start()
    {
        _position = transform.position;
    }
    
    void Update()
    {
        
        var x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed.x;
        var z = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed.z;
        var y = Input.GetAxisRaw("UpDown") * Time.deltaTime * speed.y;
        
        var _transform = transform;
        
        if(!PauseMenu.IsOpen)
            _position += _transform.right * x + _transform.forward * z + Vector3.up * y;
        
        _transform.position = Vector3.Lerp(_transform.position, _position, lerpDelta * Time.deltaTime);
    }
}
