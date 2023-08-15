using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSync : MonoBehaviourPunCallbacks, IPunObservable
{
    public new GameObject camera;
    
    public GameObject malePlayerModel;
    public GameObject femalePlayerModel;
    
    private PlayerMovement _playerMovement;
    private CameraMovement _cameraMovement;
    private GameObject _playermodel;
    
    private Vector3 _networkPosition;
    private Quaternion _networkRotation;
    
    void Start()
    {
        // if this is my player...
        if (photonView.IsMine)
        {
            var isMale = false;//Random.Range(0.0f, 1.0f) < 0.5f;
            photonView.RPC("SetPlayerModel", RpcTarget.OthersBuffered, isMale);
            GetComponent<DancerMortality>().playermodelInfo = (isMale? malePlayerModel : femalePlayerModel).GetComponent<PlayermodelInfo>();
            GameUI.instance.IndicateGender(isMale);
            
            var followPlayer = GameObject.FindGameObjectWithTag("Skybox").GetComponent<FollowPlayer>();
            followPlayer.player = transform;
            followPlayer.Init();
        }
        // otherwise...
        else
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<PlayerMovement>());
            Destroy(GetComponent<CameraMovement>());
            Destroy(GetComponent<PlayerKill>());
            camera.SetActive(false);
        }
    }

    [PunRPC]
    public void SetPlayerModel(bool isMale)
    {
        _playermodel = isMale ? malePlayerModel : femalePlayerModel;
        if (!photonView.IsMine)
        {
            _playermodel.SetActive(true);
        }
        GetComponent<DancerMortality>().playermodelInfo = _playermodel.GetComponent<PlayermodelInfo>();
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
