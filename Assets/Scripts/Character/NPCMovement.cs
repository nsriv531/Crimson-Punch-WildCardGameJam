using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NPCMovement : MonoBehaviour
{
    public float closeEnoughDistance = 0.1f;
    public float movementSpeed = 1f;
    public float rotationSpeed = 1f;
    public float glanceSpeed = 2f;
    
    private NavMeshAgent _agent;
    private NavMeshPath _path;
    private int _currentPathIndex = 0;
    private NPCState _state = NPCState.SelectNewAction; // 0 = uninitialised, 1 = walking to new position, 2 = idling

    private Vector3 _idleTargetLookPosition;
    
    public enum NPCState
    {
        SelectNewAction,
        Moving,
        Idling
    }
    
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        
        _path = new NavMeshPath();
    }

    void DecideNewPath()
    {
        var navigationPosition = Random.insideUnitSphere * 20f + transform.position;
        NavMesh.SamplePosition(navigationPosition, out NavMeshHit hit, 20f, 1);
        if (_agent.CalculatePath(hit.position, _path))
        {
            _state = NPCState.Moving;
            _currentPathIndex = 0;
        }
        else
        {
            Idle();
        }
    }

    void Idle()
    {
        var pt = Random.insideUnitCircle;
        _idleTargetLookPosition = transform.position + new Vector3(pt.x, 0.0f, pt.y);
        _state = NPCState.Idling;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(_state == NPCState.SelectNewAction)
        {
            if (Random.Range(0.0f, 1.0f) < 0.6f)
            {
                Idle();
            }
            else
            {
                DecideNewPath();
            }
            return;
        }
        
        if(_state == NPCState.Moving)
        {
            // Corrected position of the current corner such that it's on the same Y plane as the character
            var yCorrectedPosition = new Vector3(_path.corners[_currentPathIndex].x, transform.position.y, _path.corners[_currentPathIndex].z);
        
            if(Vector3.Distance(transform.position, yCorrectedPosition) < closeEnoughDistance)
            {
                _currentPathIndex++;
                if (_currentPathIndex >= _path.corners.Length)
                {
                    _state = NPCState.SelectNewAction;
                }
            }
            else
            {
                var toTarget = yCorrectedPosition - transform.position;
                transform.forward = Vector3.RotateTowards(transform.forward, toTarget, rotationSpeed * Time.deltaTime, 0f);
            
                if(Vector3.Angle(transform.forward, toTarget) < 5f)
                    transform.position = Vector3.MoveTowards(transform.position, yCorrectedPosition, movementSpeed * Time.deltaTime);
            }

            return;
        }
        
        if(_state == NPCState.Idling)
        {
            var toTarget = _idleTargetLookPosition - transform.position;
            
            if (Vector3.Angle(transform.forward, toTarget) < 5f)
            {
                _state = NPCState.SelectNewAction;
                return;
            }
            
            transform.forward = Vector3.Slerp(transform.forward, toTarget, glanceSpeed * Time.deltaTime);
        }
        
    }
}
