using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    PhotonView pv;

    float hAxis;
    float vAxis;
    Vector3 moveVec;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        if(pv.IsMine)
        {
            LobbyUIManager.Instance.photonManager.myPlayer = pv;
        }
    }

    private void OnEnable()
    {
        if (!LobbyUIManager.Instance.photonManager.playerList.Contains(this.gameObject))
        {
            pv.RPC("AddMeToLIst", RpcTarget.AllBuffered);
        }
    }


    void Update()
    {
        if(pv.IsMine)
        {
            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");

            Move();
        }
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        transform.position += moveVec * 1 * Time.deltaTime; // 1 은 스피드
    }

    [PunRPC]
    void AddMeToLIst()
    {
        if (!LobbyUIManager.Instance.photonManager.playerList.Contains(this.gameObject))
        {
            LobbyUIManager.Instance.photonManager.playerList.Add(this.gameObject);
        }
    }
}
