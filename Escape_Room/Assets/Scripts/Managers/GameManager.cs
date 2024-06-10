using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : Singleton<GameManager>
{
    [Header("Manager")]
    public PhotonManager photonManager;
    public UIManager uiManager;

    [Header("Player")]
    public List<Character_Controller> playerList;

    [Header("Killer")]
    public GameObject killerPrefab;
    public int killerSpawnTime = 600; // 10min
    public Transform killerSpawnPoint;

    public bool isSpawn = false;

    void Start()
    {
        photonManager = FindObjectOfType<PhotonManager>();
        //InvokeRepeating("Player_Check", 5, 5);
    }

    void Update()
    {
        if (!isSpawn)
        {
            if (uiManager.playTime <= killerSpawnTime)
            {
                Killer_Spawn();
            }
        }
    }

    void Player_Check()
    {
        playerList.Clear();
        playerList.Add(FindObjectOfType<Character_Controller>());
    }

    void Killer_Spawn()
    {
        isSpawn = true;

        GameObject killer = Instantiate(killerPrefab, killerSpawnPoint.position, killerSpawnPoint.rotation);
        killer.GetComponent<Killer>().gameManager = this;

        InvokeRepeating("Player_Check", 5, 5);
    }
}
