using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Killer : MonoBehaviour
{
    public GameManager gameManager;
    //플레이어 인식 임시작업
    public GameObject target;

    private NavMeshAgent nma;

    private Animator animator;

    private float attackRange = 2f;

    private bool isFind = false;
    private bool isAtk = false;

    private void Start()
    {
        Killer_Init();
    }

    private void Killer_Init()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        animator = GetComponent<Animator>();
        animator.SetBool("isWalk", false);

        nma = GetComponent<NavMeshAgent>();
        nma.speed = 10f;
        nma.angularSpeed = 50f;
        nma.acceleration = 1f;
        nma.stoppingDistance = 2f;
    }

    private void Update()
    {
        Killer_Find();
        Killer_Move();
        //Killer_Attack();
    }
        
    private void Killer_Find()
    {
        if (!isFind)
        {
        target = gameManager.playerList[0].gameObject;
        isFind = true;
        }
    }

    private void Killer_Move()
    {
        if (isFind)
        {
            nma.enabled = true;
            nma.SetDestination(target.transform.position);
            animator.SetBool("isWalk", true);
        } else
        {
            nma.SetDestination(transform.position);
        }
    }

    private void Killer_Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
        {

            animator.SetTrigger("isAtk");
        }
    }
}
