using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocalPlayerHelper : MonoBehaviour
{
    public UnityEvent onLocalPlayerFound = new UnityEvent();

    [Header("Debugging")]
    [ReadOnly, SerializeField] private bool _localPlayerFound = false;

    [HideInInspector] public bool localPlayerFound { get => _localPlayerFound; private set => _localPlayerFound = value; }

    

    void Update()
    {
        if (!localPlayerFound)
        {
            try
            {
                localPlayerFound = LocalPlayer.gameObject != null;
                if (localPlayerFound) onLocalPlayerFound.Invoke();
            }
            catch (System.Exception e)
            {
                if (!(e is LocalPlayer.LocalPlayerNotFoundException ||
                    e is LocalPlayer.LocalClientNotFoundException ||
                    e is LocalPlayer.NetworkManagerNotFoundException)) throw e;
            }
        }
    }

    public void SetLocalPlayerActive(bool value)
    {
        Character localCharacter = LocalPlayer.gameObject.GetComponent<Character>();
        //Debug.Log(localCharacter);
        if (localCharacter != null) localCharacter.gameObject.SetActive(value);
    }
}
