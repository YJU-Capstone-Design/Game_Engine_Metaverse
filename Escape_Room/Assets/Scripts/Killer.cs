using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Killer : MonoBehaviour
{
    public GameObject gameManager;
    //플레이어 인식 임시작업
    public GameObject target;

    private NavMeshAgent nma;

    private bool isFind = false;
    private bool isAtk = false;

    private void Start()
    {
        Killer_Init();
    }

    private void Killer_Init()
    {
        gameManager = FindObjectOfType<GameObject>().GetComponent<GameObject>();
        nma = GetComponent<NavMeshAgent>();
        nma.enabled = false;
    }

    private void Update()
    {
        Killer_Find();
        Killer_Move();
        Killer_Attack();
    }
        
    private void Killer_Find()
    {
        if (!isFind)
        {
        target = gameManager.playerList;
        isFind = true;
        }
    }

    private void Killer_Move()
    {
        if (isFind)
        {
            nma.enabled = true;
            nma.SetDestination(target.transform.position);
        } else
        {
            nma.SetDestination(transform.position);
        }
    }

    private void Killer_Attack()
    {

    }
}
