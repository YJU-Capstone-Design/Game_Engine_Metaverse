using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Killer
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision obj)
    {
        if (isAtk)
        {
            if (obj.transform.gameObject.CompareTag("Player"))
            {
                Destroy(obj.transform.gameObject);
                Debug.Log("Destroy Player");
            }
        }
    }
}
