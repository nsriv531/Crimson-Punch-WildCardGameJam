using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NPCMovementPUN : MonoBehaviourPunCallbacks
{
    public float closeEnoughDistance = 0.1f;
    public float movementSpeed = 1f;
    public float rotationSpeed = 1f;
    public float glanceSpeed = 2f;
    
    private NavMeshAgent _agent;
    private NavMeshPath _path;
    
    [Header("Debugging")]
    [SerializeField] private bool _verbose = false;
    [ReadOnly, SerializeField] private int _currentPathIndex = 0;
    [ReadOnly, SerializeField] private NPCState _state = NPCState.SelectNewAction; // 0 = uninitialised, 1 = walking to new position, 2 = idling
    [ReadOnly, SerializeField] private Vector3 _idleTargetLookPosition;

    private Vector3[] _pathCorners;

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
        var navigationPosition = Random.insideUnitSphere.normalized * 20f + transform.position;
        photonView.RPC("ReceiveNewPath", RpcTarget.AllBufferedViaServer, navigationPosition);
    }

    [PunRPC]
    void ReceiveNewPath(Vector3 navigationPosition)
    {
        var success = NavMesh.SamplePosition(navigationPosition, out NavMeshHit hit, 20f, 1);

        if (!success) return;
        
        // Only use finite positions
        if (!float.IsFinite(hit.position.x) || !float.IsFinite(hit.position.y) || !float.IsFinite(hit.position.z))
        {
            Debug.LogWarning("Non-finite position " + hit.position, gameObject);
            return;
        }

        if (_path != null && _agent.CalculatePath(hit.position, _path))
        {
            //state.Value = NPCState.Moving;
            _state = NPCState.Moving;
            _currentPathIndex = 0;
        }
        else
        {
            Idle();
            return;
        }

        _pathCorners = _path.corners;
    }
    
    void Idle()
    {
        // SyncTransform();
        photonView.RPC("BeginIdling", RpcTarget.AllBufferedViaServer, Random.insideUnitCircle);
    }
    
    void SyncTransform()
    {
        photonView.RPC("RecieveTransform", RpcTarget.AllViaServer, transform.position, transform.rotation);
    }

    [PunRPC]
    void RecieveTransform(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }
    
    [PunRPC]
    void BeginIdling(Vector2 idlePt)
    {
        _idleTargetLookPosition = transform.position + new Vector3(idlePt.x, 0.0f, idlePt.y);
        _state = NPCState.Idling;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (_state == NPCState.SelectNewAction)
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
        }
        
        if (_state == NPCState.Moving)
        {
            // Corrected position of the current corner such that it's on the same Y plane as the character
            var yCorrectedPosition = new Vector3(_pathCorners[_currentPathIndex].x, transform.position.y, _pathCorners[_currentPathIndex].z);
            if (Vector3.Distance(transform.position, yCorrectedPosition) < closeEnoughDistance)
            {
                _currentPathIndex++;
                if (_currentPathIndex >= _pathCorners.Length)
                {
                    _state = NPCState.SelectNewAction;
                }
            }
            else
            {
                var toTarget = yCorrectedPosition - transform.position;
                MoveToTarget(toTarget);
            }
            
            return;
        }
        
        if (_state == NPCState.Idling)
        {
            var toTarget = _idleTargetLookPosition - transform.position;
            if (Vector3.Angle(transform.forward, toTarget) < 5f && photonView.IsMine)
            {
                _state = NPCState.SelectNewAction;
                return;
            }
            transform.forward = Vector3.Slerp(transform.forward, toTarget, glanceSpeed * Time.deltaTime);
        }
        
    }

    private void MoveToTarget(Vector3 position)
    {
        if (_verbose) Debug.Log("Moving to target " + position, gameObject);
        transform.forward = Vector3.RotateTowards(transform.forward, position, rotationSpeed * Time.deltaTime, 0f);

        if (Vector3.Angle(transform.forward, position) < 5f)
            transform.position = Vector3.MoveTowards(transform.position, _pathCorners[_currentPathIndex], movementSpeed * Time.deltaTime);
    }
    
}
