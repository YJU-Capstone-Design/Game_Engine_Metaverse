using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Body : MonoBehaviour
{
    public Character_Controller parent;
    public Weapon weapon;

    private void Start()
    {
        parent = this.transform.parent.gameObject.GetComponent<Character_Controller>();
        weapon = GameObject.Find("Weapon").GetComponent<Weapon>();
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
