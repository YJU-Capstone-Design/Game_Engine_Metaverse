using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Body : MonoBehaviour
{
    public Character_Controller parent;
    public GameObject weapon;

    private void Start()
    {
        parent = this.transform.parent.gameObject.GetComponent<Character_Controller>();
    }

    private void OnCollisionEnter(Collision obj)
    {
        
        if (this.gameObject.activeSelf == true)
        {
            if (obj.gameObject == weapon)
            {
                parent.playerLife -= 1;
            }
        }
    }
}
