using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Killer : MonoBehaviour
{
    [Header("Component")]
    public GameManager gameManager;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent nma;

    [Header("Setting")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float sphereRadius = 5f;
    [SerializeField] private float findRange = 45f;

    [Header("Target")]
    [SerializeField] private GameObject target;
    public Vector3[] wanderPosition = new Vector3[4] { new Vector3(95f, -0.5f, -18f), new Vector3(92.5f, -0.5f, 0f), new Vector3(77.5f, -0.5f, 0f), new Vector3(77.5f, -0.5f, 10f) };

    [Header("State")]
    // private bool isFind = false;
    // private bool isWalk = false;
    protected bool isAtk = false;
    private bool isWander = false;

    private void Start()
    {
        Killer_Init();
    }

    private void Killer_Init()
    {
        // gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        animator = GetComponent<Animator>();
        animator.SetBool("isWalk", false);

        nma = GetComponent<NavMeshAgent>();
        nma.speed = 5f;
        nma.angularSpeed = 120f;
        nma.acceleration = 8f;
        nma.stoppingDistance = 1f;

        weapon = GetComponentInChildren<Weapon>().gameObject;
    }

    private void Update()
    {
        Killer_Find();
        Killer_Move();
        Killer_Attack();
    }
        
    private void Killer_Find()
    {
        // Raycast 사용을 위한 시작점 및 방향 설정
        Vector3 rayStart = transform.position;
        Vector3 rayDir = transform.forward;

        Quaternion leftRot = Quaternion.Euler(0, -findRange * 0.5f, 0); // 왼쪽 각도 최댓값
        Vector3 leftDir = leftRot * rayDir;
        float leftRad = Mathf.Acos(Vector3.Dot(rayDir, leftDir));
        float leftDeg = -(Mathf.Rad2Deg * leftRad);

        Quaternion rightRot = Quaternion.Euler(0, findRange * 0.5f, 0); // 오른쪽 각도 최댓값
        Vector3 rightDir = rightRot * rayDir;
        float rightRad = Mathf.Acos(Vector3.Dot(rayDir, rightDir));
        float rightDeg = Mathf.Rad2Deg * rightRad;

        // Debug.DrawRay
        Debug.DrawRay(rayStart, rayDir * sphereRadius, Color.red);
        Debug.DrawRay(rayStart, leftDir * sphereRadius, Color.green);
        Debug.DrawRay(rayStart, rightDir * sphereRadius, Color.blue);

        // 거리는 0이고 구의 크기가 30f인 범위 생성
        RaycastHit[] hits = Physics.SphereCastAll(rayStart, sphereRadius, rayDir, 0f, LayerMask.GetMask("Player"));

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.transform.gameObject);
            // 특정 Player가 Killer의 인식 범위 안에 들어올 경우
            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("CompareTag Player");
                GameObject hitPlayer = hit.transform.gameObject;
                // 해당 Player의 방향벡터값 계산
                Vector3 hitDir = (hitPlayer.transform.position - rayStart).normalized;
                float hitRad = Mathf.Acos(Vector3.Dot(rayDir, hitDir));
                float hitDeg = Mathf.Rad2Deg * hitRad;

                // 해당 Player가 Killer의 시야각 내부에 위치할 경우
                if (hitDeg >= leftDeg && hitDeg <= rightDeg)
                {
                    // 해당 Player를 Target으로 설정할 지에 대한 코드
                    if (target != null)
                    {
                        // 설정된 Target이 있을 경우 해당 Player와 Target의 거리 비교
                        // Player와 Killer 간 거리가 더 가까운 Player를 Target으로 재설정 
                        float targetDist = Vector3.Distance(rayStart, target.transform.position);
                        float hitDist = Vector3.Distance(rayStart, hitPlayer.transform.position);

                        if (hitDist < targetDist)
                        {
                            target = hitPlayer;
                        }
                    }
                    else
                    {
                        // 설정된 Target이 없을 경우 해당 Player를 Target으로 지정
                        target = hitPlayer;
                    }
                }
                else
                {
                    // 범위 내에 Player가 존재하나 설정된 Target이 없을 경우 해당 Player를 바라보도록 설정
                    // if (target == null)
                    // {
                    // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(hitDir), Time.deltaTime * 2.5f);
                    // }
                }
            }
            else Debug.Log("에바임");
        }
    }

    private void Killer_Move()
    {
        if (target != null)
        {
            nma.enabled = true;
            nma.SetDestination(target.transform.position);
            animator.SetBool("isWalk", true);
        } else
        {
            nma.SetDestination(transform.position);
            animator.SetBool("isWalk", false);
        }
    }

    private void Killer_Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
        {
            if (hit.transform.gameObject == target)
            {
                isAtk = true;
                animator.SetTrigger("isATK");
                animator.SetBool("isWalk", false);
            }
        }
    }
}
