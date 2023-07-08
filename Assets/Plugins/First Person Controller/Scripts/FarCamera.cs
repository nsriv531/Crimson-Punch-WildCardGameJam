using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarCamera : MonoBehaviour {

	void Update () {
        GetComponent<Camera>().fieldOfView = Camera.main.fieldOfView;
	}
}
