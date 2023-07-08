using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerHelper : MonoBehaviour
{
    public void SetLocalPlayerActive(bool value)
    {
        Character localCharacter = LocalPlayer.gameObject.GetComponent<Character>();
        //Debug.Log(localCharacter);
        if (localCharacter != null) localCharacter.gameObject.SetActive(value);
    }
}
