using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        int choice = Random.Range(0, transform.childCount - 1);
        transform.GetChild(choice).gameObject.SetActive(true);
    }
}
