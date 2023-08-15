using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeterministicNPC : MonoBehaviour
{
    private static int _globalSeed;
    private static bool _globalInitialised = false;
    private static int _npcCount = 0;

    public static void InitialiseGlobal(int seed = 0)
    {
        _npcCount = 0;
        _globalSeed = seed;
        _globalInitialised = true;
        Debug.Log("Initialised global with seed: " + seed);
    }

    public float movementSpeed;
    public float rotationSpeed;
    public float closeEnoughDistanceSquared = 0.01f;

    private Vector3[] _path;
    private int _currentPathIndex = 0;
    private bool _waitingForPath = true;
    
    private int _npcID;
    private System.Random _rng;
    
    void Start()
    {
        if (!_globalInitialised) throw new System.Exception("NPC Behaviour not globally initialised");
        _npcID = _npcCount;
        _npcCount++;
        _rng = new System.Random(_globalSeed + _npcID);
        Debug.Log("Spawned NPC at "  + transform.position + ", with seed: " + _globalSeed + _npcID);
        PathManager.ForcePathCalculation(transform.position, GameManager.instance.GetRandomPointOnNavMesh(_rng), OnPathCalculated);
    }

    void Update()
    {
        if (_waitingForPath) return;
        
        if (_currentPathIndex < _path.Length)
        {
            var _transform = transform;
            var target = _path[_currentPathIndex];
            var direction = target - _transform.position;
            var directionYCorrected = new Vector3(direction.x, 0, direction.z);
            var distance = direction.sqrMagnitude;
            if (distance < closeEnoughDistanceSquared)
            {
                _currentPathIndex++;
            }
            else
            {
                _transform.SetPositionAndRotation(
                    Vector3.MoveTowards(_transform.position, target, movementSpeed * Time.deltaTime), 
                    Quaternion.RotateTowards(_transform.rotation, Quaternion.LookRotation(directionYCorrected), rotationSpeed * Time.deltaTime));
            }
        }
        else
        {
            _waitingForPath = true;
            PathManager.RequestPathCalculation(transform.position, GameManager.instance.GetRandomPointOnNavMesh(_rng), OnPathCalculated);
        }
    }

    void OnPathCalculated(Vector3[] path)
    {
        _currentPathIndex = 0;
        _path = path;
        if (path.Length == 0)
        {
            Debug.LogError("Path calculation failed (length is zero)");
        }
        else
        {
            _waitingForPath = false;
        }
    }
}
