using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Manager")]
    public PhotonManager photonManager;

    [Header("Player")]
    public List<Player_Test> playerList;

    [Header("Killer")]
    public GameObject killerPrefab;
    public Transform killerSpawnPoint;

    public bool isSpawn = false;

    void Start()
    {
        photonManager = FindObjectOfType<PhotonManager>();
        InvokeRepeating("Player_Check", 5, 5);
    }

    void Update()
    {
    }

    void Player_Check()
    {
        playerList.Clear();
        playerList.Add(FindObjectOfType<Player_Test>());

        playerList.OrderBy(Player_Test => Player_Test.lifePoint).ToList();

        Debug.Log("playerList[0].lifePoint = " + playerList[0].lifePoint);

        if (!isSpawn)
        {
            if (playerList[0].lifePoint == 0)
            {
                Killer_Spawn();
            }
        }
    }

    void Killer_Spawn()
    {
        GameObject Killer = Instantiate(killerPrefab, killerSpawnPoint.position, killerSpawnPoint.rotation);
        Killer.GetComponent<Killer>().gameManager = this;
        // Killer.GetComponent<NavMeshAgent>().enabled = false;
        isSpawn = true;
    }
}
