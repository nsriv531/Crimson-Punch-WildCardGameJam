using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attach this Component to a GameObject to use as the position to instantiate
/// Characters.
/// </summary>

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private bool randomRotation = true;

    [SerializeField] private UnityEvent onSpawn = new UnityEvent();

    /// <summary>
    /// Move the given Character to the position of this Transform
    /// </summary>
    public void Spawn(Character character)
    {
        character.transform.SetPositionAndRotation(transform.position, GetRotation());
        onSpawn.Invoke();
    }

    /// <summary>
    /// Instantiate a copy of the GameObject the given Character is attached to at this Transform's position
    /// </summary>
    public void SpawnCopy(Character character)
    {
        if (character.gameObject == LocalPlayer.gameObject) throw new System.Exception("Spawning a copy of the local player is not allowed.");

        GameObject newGO = null;
        newGO = Instantiate(character.gameObject, transform.position, GetRotation(), null);

        NetworkObject networkObject = newGO.GetComponent<NetworkObject>();
        if (networkObject != null) networkObject.Spawn();

        onSpawn.Invoke();
    }

    private Quaternion GetRotation()
    {
        Quaternion rotation = transform.rotation;
        if (randomRotation)
        {
            rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
        }
        return rotation;
    }
}
