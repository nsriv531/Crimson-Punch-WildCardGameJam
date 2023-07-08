using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class NetworkNPCMovement : MonoBehaviour
{
    private Character _character;
    [HideInInspector] public Character character
    {
        get
        {
            if (_character == null) _character = GetComponent<Character>();
            return _character;
        }
    }


}
