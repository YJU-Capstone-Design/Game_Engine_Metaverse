using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Test : MonoBehaviour
{
    public int lifePoint = 3;

    void Update()
    {
        Life_Down();
    }
    
    void Life_Down()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (lifePoint > 0)
            {
                lifePoint--;
            }
        }
    }
}
