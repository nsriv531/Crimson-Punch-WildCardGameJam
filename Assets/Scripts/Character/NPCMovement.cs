using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Character))]
public class NPCMovement : NetworkBehaviour
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

    [HideInInspector] public NetworkVariable<NPCState> state = new NetworkVariable<NPCState>(NPCState.SelectNewAction);

    private Vector3[] pathCorners;

    public enum NPCState
    {
        SelectNewAction,
        Moving,
        Idling
    }

    private Character _character;
    [HideInInspector] public Character character
    {
        get
        {
            if (_character == null) _character = GetComponent<Character>();
            return _character;
        }
    }
    
    void Start()
    {
        // Defer the stuff done here to OnNetworkSpawn
        if (NetworkManager.Singleton != null) return;

        Initialize();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Initialize();

        // If it's a non-host client, synchronize now
        if (IsOwner && IsClient && !IsHost)
        {
            SynchronizeServerRpc(character.clientId);
        }

        //state.OnValueChanged += (NPCState previous, NPCState current) => _state = current;
    }


    private void Initialize()
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
            //state.Value = NPCState.Moving;
            _state = NPCState.Moving;
            _currentPathIndex = 0;
        }
        else
        {
            Idle();
        }

        pathCorners = _path.corners;

        // Synchronize other non-server clients with the new path
        SynchronizeClientRpc(transform.position, transform.rotation, pathCorners, (int)_state, _idleTargetLookPosition, _currentPathIndex);
    }

    void Idle()
    {
        var pt = Random.insideUnitCircle;
        _idleTargetLookPosition = transform.position + new Vector3(pt.x, 0.0f, pt.y);
        //state.Value = NPCState.Idling;
        _state = NPCState.Idling;

        // Synchronize the new path with the server
        SynchronizeClientRpc(transform.position, transform.rotation, pathCorners, (int)_state, _idleTargetLookPosition, _currentPathIndex);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (IsServer)
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
            if (IsServer)
            {
                // Corrected position of the current corner such that it's on the same Y plane as the character
                var yCorrectedPosition = new Vector3(pathCorners[_currentPathIndex].x, transform.position.y, pathCorners[_currentPathIndex].z);
                if (Vector3.Distance(transform.position, yCorrectedPosition) < closeEnoughDistance)
                {
                    _currentPathIndex++;
                    if (_currentPathIndex >= pathCorners.Length)
                    {
                        _state = NPCState.SelectNewAction;
                    }
                    SynchronizeClientRpc(transform.position, transform.rotation, pathCorners, (int)_state, _idleTargetLookPosition, _currentPathIndex);
                }
                else
                {
                    var toTarget = yCorrectedPosition - transform.position;
                    MoveToTarget(toTarget);
                }
            }
            else
            {
                if (_currentPathIndex < pathCorners.Length)
                {
                    var yCorrectedPosition = new Vector3(pathCorners[_currentPathIndex].x, transform.position.y, pathCorners[_currentPathIndex].z);
                    if (Vector3.Distance(transform.position, yCorrectedPosition) >= closeEnoughDistance)
                    {
                        var toTarget = yCorrectedPosition - transform.position;
                        MoveToTarget(toTarget);
                    }
                }
            }
            return;
        }
        
        if (_state == NPCState.Idling)
        {
            var toTarget = _idleTargetLookPosition - transform.position;
            if (IsServer)
            {
                if (Vector3.Angle(transform.forward, toTarget) < 5f)
                {
                    _state = NPCState.SelectNewAction;
                    SynchronizeClientRpc(transform.position, transform.rotation, pathCorners, (int)_state, _idleTargetLookPosition, _currentPathIndex);
                    return;
                }
            }
            transform.forward = Vector3.Slerp(transform.forward, toTarget, glanceSpeed * Time.deltaTime);
        }
        
    }

    private void MoveToTarget(Vector3 position)
    {
        if (_verbose) Debug.Log("Moving to target " + position, gameObject);
        transform.forward = Vector3.RotateTowards(transform.forward, position, rotationSpeed * Time.deltaTime, 0f);

        if (Vector3.Angle(transform.forward, position) < 5f)
            transform.position = Vector3.MoveTowards(transform.position, pathCorners[_currentPathIndex], movementSpeed * Time.deltaTime);
    }




    #region Networking
    /// <summary>
    /// Called by server only, run by the clients only.
    /// </summary>
    [ClientRpc]
    private void SynchronizeClientRpc(Vector3 currentPosition, Quaternion currentRotation, Vector3[] corners, int state, Vector3 idleTargetLookPosition, int currentPathIndex, ClientRpcParams clientRpcParams = default)
    {
        if (!IsClient) throw new System.Exception("ClientRPCs can only be run by clients");

        // Synchronize the transforms in case of network latency
        transform.SetPositionAndRotation(currentPosition, currentRotation);

        // Synchronize the path
        pathCorners = corners;

        // Sync the state
        _state = (NPCState)state;

        _idleTargetLookPosition = idleTargetLookPosition;
        _currentPathIndex = currentPathIndex;

        if (_verbose) Debug.Log("Synchronized clients " + clientRpcParams.Send.TargetClientIds, gameObject);
    }


    /// <summary>
    /// Called by clients only, run by server only.
    /// </summary>
    [ServerRpc]
    private void SynchronizeServerRpc(ulong clientId)
    {
        if (!IsServer) throw new System.Exception("ServerRPCs can only be called by the server");

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        // Synchronize the client who called this function
        if (_verbose) Debug.Log("Attempting to synchronize client " + clientId + " to the server", gameObject);
        SynchronizeClientRpc(transform.position, transform.rotation, pathCorners, (int)_state, _idleTargetLookPosition, _currentPathIndex, clientRpcParams);
    }

    #endregion
}
