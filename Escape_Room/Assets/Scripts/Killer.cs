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
        // Raycast ����� ���� ������ �� ���� ����
        Vector3 rayStart = transform.position;
        Vector3 rayDir = transform.forward;

        Quaternion leftRot = Quaternion.Euler(0, -findRange * 0.5f, 0); // ���� ���� �ִ�
        Vector3 leftDir = leftRot * rayDir;
        float leftRad = Mathf.Acos(Vector3.Dot(rayDir, leftDir));
        float leftDeg = -(Mathf.Rad2Deg * leftRad);

        Quaternion rightRot = Quaternion.Euler(0, findRange * 0.5f, 0); // ������ ���� �ִ�
        Vector3 rightDir = rightRot * rayDir;
        float rightRad = Mathf.Acos(Vector3.Dot(rayDir, rightDir));
        float rightDeg = Mathf.Rad2Deg * rightRad;

        // Debug.DrawRay
        Debug.DrawRay(rayStart, rayDir * sphereRadius, Color.red);
        Debug.DrawRay(rayStart, leftDir * sphereRadius, Color.green);
        Debug.DrawRay(rayStart, rightDir * sphereRadius, Color.blue);

        // �Ÿ��� 0�̰� ���� ũ�Ⱑ 30f�� ���� ����
        RaycastHit[] hits = Physics.SphereCastAll(rayStart, sphereRadius, rayDir, 0f, LayerMask.GetMask("Player"));

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.transform.gameObject);
            // Ư�� Player�� Killer�� �ν� ���� �ȿ� ���� ���
            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("CompareTag Player");
                GameObject hitPlayer = hit.transform.gameObject;
                // �ش� Player�� ���⺤�Ͱ� ���
                Vector3 hitDir = (hitPlayer.transform.position - rayStart).normalized;
                float hitRad = Mathf.Acos(Vector3.Dot(rayDir, hitDir));
                float hitDeg = Mathf.Rad2Deg * hitRad;

                // �ش� Player�� Killer�� �þ߰� ���ο� ��ġ�� ���
                if (hitDeg >= leftDeg && hitDeg <= rightDeg)
                {
                    // �ش� Player�� Target���� ������ ���� ���� �ڵ�
                    if (target != null)
                    {
                        // ������ Target�� ���� ��� �ش� Player�� Target�� �Ÿ� ��
                        // Player�� Killer �� �Ÿ��� �� ����� Player�� Target���� �缳�� 
                        float targetDist = Vector3.Distance(rayStart, target.transform.position);
                        float hitDist = Vector3.Distance(rayStart, hitPlayer.transform.position);

                        if (hitDist < targetDist)
                        {
                            target = hitPlayer;
                        }
                    }
                    else
                    {
                        // ������ Target�� ���� ��� �ش� Player�� Target���� ����
                        target = hitPlayer;
                    }
                }
                else
                {
                    // ���� ���� Player�� �����ϳ� ������ Target�� ���� ��� �ش� Player�� �ٶ󺸵��� ����
                    // if (target == null)
                    // {
                    // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(hitDir), Time.deltaTime * 2.5f);
                    // }
                }
            }
            else Debug.Log("������");
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
