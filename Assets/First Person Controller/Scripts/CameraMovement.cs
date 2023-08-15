using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static float sensitivity = 1.0f;

    public GameObject player;
    public GameObject camera;
    
    private Vector2 fakeLocation;

    [Range(0.01f, 10f)]
    public float xSens = 1;
    [Range(0.01f, 10f)]
    public float ySens = 1;

    public float yLimit = 90;

    public void Start()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
        if (!camera && GetComponent<Camera>()) camera = GetComponent<Camera>().gameObject;
    }

    public void Update()
    {
        if (!camera) camera = Camera.main.gameObject;

        if (PauseMenu.IsOpen) return;
        
        fakeLocation += new Vector2(Input.GetAxisRaw("Mouse X") * xSens * sensitivity, -Input.GetAxisRaw("Mouse Y") * ySens * sensitivity);
        fakeLocation = new Vector2(fakeLocation.x, Mathf.Clamp(fakeLocation.y, -yLimit, yLimit));

        player.transform.localRotation = Quaternion.Euler(
            player.transform.localRotation.eulerAngles.x,
            fakeLocation.x,
            player.transform.localRotation.eulerAngles.z);

        camera.transform.localRotation = Quaternion.Euler(
            fakeLocation.y,
            camera.transform.localRotation.eulerAngles.y,
            camera.transform.localRotation.eulerAngles.z);
    }
}