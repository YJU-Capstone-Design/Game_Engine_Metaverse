using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : Singleton<GameManager>
{
    [Header("Manager")]
    public PhotonManager photonManager;

    [Header("Player")]
    public List<Character_Controller> playerList;

    [Header("Killer")]
    public GameObject killerPrefab;
    public int killerSpawnTime = 1200;
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
            if (UIManager.Instance.playTime <= killerSpawnTime)
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
        GameObject Killer = Instantiate(killerPrefab, killerSpawnPoint.position, killerSpawnPoint.rotation);
        Killer.GetComponent<Killer>().gameManager = this;
        isSpawn = true;
    }
}
