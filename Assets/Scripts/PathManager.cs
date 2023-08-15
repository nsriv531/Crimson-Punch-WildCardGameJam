using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathManager : MonoBehaviour
{
    private class PathRequest
    {
        public Vector3 Start { get; private set; }
        public Vector3 End { get; private set; }
        public System.Action<Vector3[]> Callback { get; private set; }
        
        public PathRequest(Vector3 start, Vector3 end, System.Action<Vector3[]> callback)
        {
            Start = start;
            End = end;
            Callback = callback;
        }
    }
    
    private static PathManager _instance;
    
    public static void RequestPathCalculation(Vector3 start, Vector3 end, System.Action<Vector3[]> callback)
    {
        _instance._pathQueue.Enqueue(new PathRequest(start, end, callback));
    }

    public static void ForcePathCalculation(Vector3 start, Vector3 end, System.Action<Vector3[]> callback)
    {
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, _instance._path))
        {
            callback(_instance._path.corners);
        }
        else
        {
            callback(new Vector3[] { });
        }
    }
    
    private Queue<PathRequest> _pathQueue;
    private NavMeshPath _path;

    void Start()
    {
        _instance = this;
        _path = new NavMeshPath();
        _pathQueue = new Queue<PathRequest>();
        StartCoroutine(PathLoop());
    }

    // Loop performs one path calculation per frame
    private IEnumerator PathLoop()
    {
        while (true)
        {
            if (_pathQueue.TryDequeue(out var pathRequest))
            {
                ForcePathCalculation(pathRequest.Start, pathRequest.End, pathRequest.Callback);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
