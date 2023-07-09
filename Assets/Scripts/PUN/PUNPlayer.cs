using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PUNPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public new GameObject camera;
    
    public GameObject malePlayerModel;
    public GameObject femalePlayerModel;
    
    private PlayerMovement _playerMovement;
    private CameraMovement _cameraMovement;
    
    private Vector3 _networkPosition;
    private Quaternion _networkRotation;
    
    void Start()
    {
        // if this is my player...
        if (photonView.IsMine)
        {   
            photonView.RPC("ShowPlayerModel", RpcTarget.OthersBuffered, Random.Range(0.0f, 1.0f) < 0.5f);
        }
        // otherwise...
        else
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<PlayerMovement>());
            Destroy(GetComponent<CameraMovement>());
            camera.SetActive(false);
            
        }
        
        
        // otherwise, everything's good
    }

    [PunRPC]
    public void ShowPlayerModel(bool isMale)
    {
        Debug.Log("Showing player model isMale = " + isMale, gameObject);
        (isMale? malePlayerModel : femalePlayerModel).SetActive(true);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // SEND DATA HERE
        if (stream.IsWriting)
        {
            Vector3 pos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        else
        // RECEIEVE DATA HERE
        {
            stream.Serialize(ref _networkPosition); 
            stream.Serialize(ref _networkRotation);
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            // You have committed a sin with lerping on deltaTime....
            Vector3 newPosition = Vector3.Lerp(transform.position, _networkPosition, 5f * Time.deltaTime);
            Quaternion newRotation = Quaternion.Lerp(transform.rotation, _networkRotation, 5f * Time.deltaTime);
            transform.SetPositionAndRotation(newPosition, newRotation);
        }
    }

}
