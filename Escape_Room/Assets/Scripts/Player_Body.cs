using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Body : MonoBehaviour
{
    public Character_Controller parent;
    public GameObject killer;
    public GameObject weapon;

    private void Start()
    {
        Debug.Log(this.transform.parent.name);
        parent = this.transform.parent.gameObject.GetComponent<Character_Controller>();
        
    }

    private void Update()
    {
        if (killer == null)
        {
            killer = FindObjectOfType<Killer>().gameObject;

        } else
        {
            if (weapon == null)
            {
                weapon = FindObjectOfType<Weapon>().gameObject;
            }
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (this.gameObject.activeSelf == true)
        {
            if (obj.gameObject == weapon)
            {
                parent.playerLife -= 1;
                Debug.Log(parent.playerLife);
            }
        }
    }
}
