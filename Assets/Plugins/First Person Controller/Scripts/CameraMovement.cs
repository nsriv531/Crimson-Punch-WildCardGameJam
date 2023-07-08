using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector2 fakeLocation;

    [Range(0.01f, 10f)]
    public float xSens = 1;
    [Range(0.01f, 10f)]
    public float ySens = 1;

    public float yLimit = 90;

    private GameObject player;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        fakeLocation += new Vector2(Input.GetAxisRaw("Mouse X") * xSens, -Input.GetAxisRaw("Mouse Y") * ySens);
        fakeLocation = new Vector2(fakeLocation.x, Mathf.Clamp(fakeLocation.y, -yLimit, yLimit));

        player.transform.localRotation = Quaternion.Euler(
            player.transform.localRotation.eulerAngles.x,
            fakeLocation.x,
            player.transform.localRotation.eulerAngles.z);

        Camera.main.transform.localRotation = Quaternion.Euler(
            fakeLocation.y,
            Camera.main.transform.localRotation.eulerAngles.y,
            Camera.main.transform.localRotation.eulerAngles.z);
    }
}