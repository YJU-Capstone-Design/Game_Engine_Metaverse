using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Player Setting")]
    public PhotonManager photonManager;

    public List<Player_Test> playerList;

    [Header("Killer Setting")]
    public GameObject killerPrefab;
    public GameObject killerSpawnPoint;

    public bool isSpawn = false;
    public bool isCheck = false;

    void Start()
    {
        photonManager = FindObjectOfType<PhotonManager>();
    }

    void Update()
    {
        if (!isSpawn) // 필드에 스폰된 킬러가 없을 경우
        {
            if (!isCheck)
            {
                // 생명 포인트가 0인 플레이어를 찾을 때까지
                StartCoroutine(Life_Check());
            }
            else
            {
                // 생명 포인트가 0인 플레이어가 생길 경우 킬러 스폰
                Killer_Spawn();
            }
        }
    }

    IEnumerator Life_Check()
    {
        playerList.Clear();
        playerList.Add(FindObjectOfType<Player_Test>());
        playerList.OrderBy(Player_Test => Player_Test.lifePoint).ToList();

        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].lifePoint == 0)
            {
                isCheck = true;
                break;
            }
        }
        yield return new WaitForSecondsRealtime(5f);
    }

    void Killer_Spawn()
    {
        GameObject Killer = Instantiate(killerPrefab, killerSpawnPoint.transform.position, killerSpawnPoint.transform.rotation);
        Killer.GetComponent<NavMeshAgent>().enabled = false;
        isSpawn = true;
    }
}
