using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Killer : MonoBehaviourPunCallbacks
{
    [Header("Component")]
    public GameManager gameManager;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent nma;
    [SerializeField] AudioSource killderAudioSource;
    [SerializeField] PhotonView pv;

    [Header("Setting")]
    public GameObject weapon;
    // [SerializeField] private float attackRange = 5f;
    [SerializeField] private float sphereRadius = 5f;
    [SerializeField] private float findRange = 120f;

    [Header("Target")]
    [SerializeField] private GameObject target;
    [SerializeField] GameObject targetBody;
    public Vector3[] wanderPosition = new Vector3[5] { new Vector3(95f, -0.5f, -18f), new Vector3(92.5f, -0.5f, 0f), new Vector3(77.5f, -0.5f, 0f), new Vector3(77.5f, -0.5f, -10.5f), new Vector3(87.5f, -0.5f, -8f) };

    [Header("State")]
    // private bool isFind = false;
    // private bool isWalk = false;
    [SerializeField] protected bool isAtk = false;
    [SerializeField] private bool isWander = false;

    [Header("Sound")]
    bool idle = false;
    [SerializeField] AudioClip walkSound;
    [SerializeField] AudioClip runSound;

    private void Start()
    {
        Killer_Init();

        killderAudioSource = AudioManager.Instance.killerAudio;
    }

    private void Killer_Init()
    {
        // gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        animator = GetComponent<Animator>();
        animator.SetBool("isWalk", false);

        nma = GetComponent<NavMeshAgent>();
        nma.speed = 3f;
        nma.angularSpeed = 120f;
        nma.acceleration = 8f;
        nma.stoppingDistance = 0.5f;

        weapon = GetComponentInChildren<Weapon>().gameObject;

        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Killer_Find();
            Killer_Move();

            pv.RPC("RPC_Move", RpcTarget.All, transform.position, transform.rotation); 
        }
        Killer_Attack();
    }

    [PunRPC]
    void RPC_Move(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    private void Killer_Find()
    {
        // Raycast ����� ���� ������ �� ���� ����
        Vector3 rayStart = transform.position + new Vector3(0, 1.5f, 0);
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

        if (hits != null)
        {
            foreach (RaycastHit hit in hits)
            {
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
                    if (target != null)
                    {
                        Vector3 targetDir = (target.transform.position - rayStart).normalized;
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * 2.5f);
                    }
                }
            }
        } else
        {
            target = null;
            isWander = false;
        }

        if (target != null)
        {
            if (target.GetComponentInChildren<Player_Body>() == null)
            {
                target = null;
                isWander = false;
            }
        }
    }

    private void Killer_Move()
    {
        nma.enabled = true;
        if (!isAtk)
        {
            if (target != null)
            {
                StopCoroutine(Wander());
                isWander = false;
                nma.speed = 5f;
                nma.SetDestination(target.transform.position);
                animator.SetBool("isWalk", true);
            }
            else
            {
                if (!isWander)
                {
                    isWander = true;
                    nma.speed = 3f;
                    StartCoroutine(Wander());
                }
            }

            if(target != null)
            {
                // ����
                // StartCoroutine(PlaySound("Run"));
            }
            else if(target == null && !idle)
            {
                // ����
                // StartCoroutine(PlaySound("Walk"));
            }
        }
    }

    private IEnumerator Wander()
    {

        for (int i = 0; i < wanderPosition.Length;)
        {
            Vector3 tfPos = new Vector3(wanderPosition[i].x, transform.position.y, wanderPosition[i].z);
            nma.SetDestination(tfPos);
            animator.SetBool("isWalk", true);
            while (Vector3.Distance(transform.position, tfPos) > 1.5f)
            {
                yield return null;
            }
            nma.SetDestination(transform.position);
            animator.SetBool("isWalk", false);
            idle = true;
            killderAudioSource.Stop();
            killderAudioSource.clip = null;
            yield return new WaitForSecondsRealtime(3f);
            idle = false;

            if (i == wanderPosition.Length - 1)
            {
                i = 0;
            }
            else
            {
                i++;
            }
        }
    }

    private void Killer_Attack()
    {
        if (target != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), transform.forward, out hit, 5f, LayerMask.GetMask("Player")))
            {
                if (hit.transform.gameObject == target && !isAtk)
                {
                    if (Vector3.Distance(transform.position, target.transform.position) < 1f && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    {
                        isAtk = true;
                        nma.SetDestination(transform.position);
                        animator.SetTrigger("isAtk");
                        animator.SetBool("isWalk", false);
                        killderAudioSource.Stop();
                        killderAudioSource.clip = null;
                    }
                }
                else if(hit.transform.gameObject == null)
                {
                    isAtk = false;
                }
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                isAtk = false;
            }
        }
        else
        {
            isAtk = false;
        }
    }

    private void OnDestroy()
    {
        // ���θ��� �ʿ��� ������ ��(������ ���� ��) �÷��̾� ���� ����
        gameManager.CancelInvoke("Player_Check");
    }

    private void OnCollisionEnter(Collision obj)
    {
        if (obj.gameObject.CompareTag("BlockLine")) {
            Physics.IgnoreCollision(obj.collider, GetComponent<Collider>());
        }
    }

    // ����
    IEnumerator PlaySound(string state)
    {
        if (state == "Walk")
        {
            if (killderAudioSource.clip != walkSound || !killderAudioSource.isPlaying)
            {
                Debug.Log("KillerWalk");
                killderAudioSource.Stop();
                killderAudioSource.clip = null;
                killderAudioSource.clip = walkSound;
                killderAudioSource.Play();
            }
        }
        else if (state == "Run")
        {
            if (killderAudioSource.clip != runSound || !killderAudioSource.isPlaying)
            {
                killderAudioSource.Stop();
                killderAudioSource.clip = null;
                killderAudioSource.clip = runSound;
                killderAudioSource.Play();
            }
        }

        yield return new WaitForSeconds(1);
    }
}
