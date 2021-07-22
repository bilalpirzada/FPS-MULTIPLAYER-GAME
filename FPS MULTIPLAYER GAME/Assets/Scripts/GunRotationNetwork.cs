using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunRotationNetwork : MonoBehaviour
{

    Vector3 realPosition;
    Quaternion realRotation;

    PhotonView PV;


    private void Awake()
    {
        PV.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

        if (PV.IsMine)
        {

        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, .1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, .1f);
        }
    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
