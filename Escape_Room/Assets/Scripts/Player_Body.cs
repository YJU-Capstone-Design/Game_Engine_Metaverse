using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Body : MonoBehaviour
{
    public Character_Controller parent;
    public GameObject killer;
    public GameObject weapon;

    PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        parent = this.transform.parent.gameObject.GetComponent<Character_Controller>();
        
    }

    private void Update()
    {   
        if (FindObjectOfType<Killer>() != null)
        {
            if (killer == null)
            {
                killer = FindObjectOfType<Killer>().gameObject;
            }
        }
        
        if (killer != null)
        {
            if (weapon == null)
            {
                weapon = killer.GetComponentInChildren<Weapon>().gameObject;
            }
        }
    }

    [PunRPC]
    void LoseLife()
    {
        parent.playerLife -= 1;
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (this.gameObject.activeSelf == true)
        {
            if (obj.gameObject == weapon)
            {
                if(pv.IsMine) 
                {
                    pv.RPC("LoseLife", RpcTarget.All);
                    UIManager.Instance.playerLife[parent.playerLife].SetActive(false);
                }
                Debug.Log("Player Life = " + parent.playerLife);
            }
        }
    }


}
