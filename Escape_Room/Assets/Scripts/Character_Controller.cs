using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character_Controller : MonoBehaviourPunCallbacks
{
    /* -------------------------------------------------- */

    [Header("Component")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PhotonManager photonManager;
    [SerializeField] private PhotonView pv;
    [SerializeField] private PhotonTransformView photonTransformView;
    [SerializeField] private UIManager uiManager;
    [SerializeField] AudioSource playerAudioSource;

    [Header("Parts")]
    // Insert "Body" Object
    [SerializeField] private GameObject player_Body;
    // Camera (1st or 3rd)
    [SerializeField] private GameObject camera_First, camera_Third;
    // Insert "Rotate_Horizontal" Object
    [SerializeField] private Transform camera_Rotation;

    [Header("Speed")]
    [SerializeField] private float speed_Walk = 10f;
    [SerializeField] private float speed_Run = 15f;
    [SerializeField] private float speed_Rotate = 1f;

    [Header("Detect")]
    [SerializeField] private float sphereRadius = 2f;
    [SerializeField] private float findRange = 45f;

    [Header("Interact")]
    [SerializeField] private GameObject detectObj;

    [Header("Status")]
    public int playerLife = 3;

    [Header("Sound")]
    [SerializeField] AudioClip walkSound;
    [SerializeField] AudioClip runSound;


    // Concealed variable
    // Player Position
    private float pos_X, pos_Z;
    // Camera Rotation
    private float rot_X, rot_Y;

    /* -------------------------------------------------- */

    private void Awake()
    {
        Player_Init();

        uiManager = LobbyUIManager.Instance.photonManager.inGameUIManager;
    }

    private void Player_Init()
    {
        photonManager = LobbyUIManager.Instance.photonManager;
        pv = GetComponent<PhotonView>();
        photonTransformView = GetComponent<PhotonTransformView>();

        if (photonView.IsMine)
        {
            LobbyUIManager.Instance.photonManager.myPlayer = photonView;

            photonView.RPC("SetName", RpcTarget.AllBuffered, LobbyUIManager.Instance.photonManager.masterName);
        }
    }

    [PunRPC]
    void SetName(string name)
    {
        gameObject.name = name;
    }

    /* -------------------------------------------------- */

    public override void OnEnable()
    {
        if (!LobbyUIManager.Instance.photonManager.playerList.Contains(this.gameObject))
        {
            photonView.RPC("AddMeToList", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void AddMeToList()
    {
        if (!LobbyUIManager.Instance.photonManager.playerList.Contains(this.gameObject))
        {
            LobbyUIManager.Instance.photonManager.playerList.Add(this.gameObject);
        }
    }

    /* -------------------------------------------------- */

    private void Start()
    {
        Camera_Setting();
        Player_Setting();

        playerAudioSource = AudioManager.Instance.playerAudio;
    }

    private void Player_Setting()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        player_Body.layer = 7; // Player Layer
    }

    private void Camera_Setting()
    {
        if (photonView.IsMine)
        {
            // Enable and setting local player's third camera 
            camera_First.SetActive(true);
            camera_Third.SetActive(false);
        }
        else
        {
            // Disable other player's camera
            camera_First.SetActive(false);
            camera_Third.SetActive(false);
        }
    }

    /* -------------------------------------------------- */

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Player code
            if (uiManager != null) // 상호 작용 중이지 않을 경우에만 움직임 가능
            {
                if (uiManager.interacting || LobbyUIManager.Instance.interacting)
                {
                    speed_Walk = 0;
                    speed_Run = 0;
                    rb.velocity = Vector3.zero;
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);

                    playerAudioSource.Stop();
                    playerAudioSource.clip = null;
                }
                else
                {
                    speed_Walk = 3f;
                    speed_Run = 6f;

                    Player_Move();

                    // Camera code
                    Camera_Change();
                    Camera_Rotate();
                }
            }
            else
            {
                Player_Move();

                // Camera code
                Camera_Change();
                Camera_Rotate();
            }

            Player_DetectObject();
            Player_InteractObject();

            Player_Die();
        }
    }

    private void Player_Move()
    {
        pos_X = Input.GetAxisRaw("Horizontal");
        pos_Z = Input.GetAxisRaw("Vertical");

        if (pos_X != 0 || pos_Z != 0) // isMove
        {
            // Direction vector of the camera
            Vector3 moveDir_Vector3 = (camera_Rotation.localRotation * Vector3.forward).normalized;
            Vector3 moveDir_Forward = moveDir_Vector3 * pos_Z;
            Vector3 moveDir_Right = Quaternion.Euler(0, 90, 0) * moveDir_Vector3 * pos_X;

            // Player move direction
            Vector3 moveDir = moveDir_Forward + moveDir_Right;
           
            player_Body.transform.localRotation = Quaternion.Slerp(player_Body.transform.rotation, Quaternion.LookRotation(moveDir.normalized), speed_Rotate);

            animator.SetBool("Walk", true);

            if (Input.GetKey(KeyCode.LeftShift)) // isRun
            {
                // Run State
                animator.SetBool("Run", true);

                if (player_Body.activeSelf == true)
                {
                    rb.velocity = moveDir.normalized * speed_Run;
                }
                else
                {
                    transform.Translate(moveDir.normalized * speed_Run * Time.deltaTime);
                }

                // 사운드
                StartCoroutine(PlaySound("Run"));
            }
            else // !isRun
            {
                // Walk State
                animator.SetBool("Run", false);

                if (player_Body.activeSelf == true)
                {
                    rb.velocity = moveDir.normalized * speed_Walk;
                }
                else
                {
                    transform.Translate(moveDir.normalized * speed_Walk * Time.deltaTime);
                }

                // 사운드
                StartCoroutine(PlaySound("Walk"));
            }
        }
        else // !isMove
        {
            // Idle State
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            rb.velocity = Vector3.zero;
            player_Body.transform.localRotation = Quaternion.Euler(new Vector3(0, camera_Rotation.localEulerAngles.y, 0));

            // 사운드
            playerAudioSource.Stop();
            playerAudioSource.clip = null;
        }
    }

    private void Player_DetectObject()
    {
        Vector3 rayStart = camera_First.transform.position;
        Vector3 rayDir = camera_First.transform.forward;

        Quaternion leftRot = Quaternion.Euler(0, -findRange * 0.5f, 0);
        Vector3 leftDir = leftRot * rayDir;
        float leftRad = Mathf.Acos(Vector3.Dot(rayDir, leftDir));
        float leftDeg = -(Mathf.Rad2Deg * leftRad);

        Quaternion rightRot = Quaternion.Euler(0, findRange * 0.5f, 0);
        Vector3 rightDir = rightRot * rayDir;
        float rightRad = Mathf.Acos(Vector3.Dot(rayDir, rightDir));
        float rightDeg = Mathf.Rad2Deg * rightRad;

        Debug.DrawRay(rayStart, rayDir * sphereRadius, Color.red);
        Debug.DrawRay(rayStart, leftDir * sphereRadius, Color.green);
        Debug.DrawRay(rayStart, rightDir * sphereRadius, Color.blue);

        RaycastHit[] hits = Physics.SphereCastAll(rayStart, sphereRadius, rayDir, 0f, LayerMask.GetMask("ActiveObject"));
        List<Transform> specificRangeHit = new List<Transform>();

        if (hits.Length < 1) { detectObj = null; }

        foreach (RaycastHit hit in hits)
        {
            //Debug.Log("Hit : " + hit.transform.gameObject.name);
            GameObject hitObj = hit.transform.gameObject;

            Vector3 hitDir = (hitObj.transform.position - rayStart).normalized;
            float hitRad = Mathf.Acos(Vector3.Dot(rayDir, hitDir));
            float hitDeg = Mathf.Rad2Deg * hitRad;

            if (hitDeg >= leftDeg && hitDeg <= rightDeg)
            {
                //Debug.Log(hit.transform.gameObject.name);

                if(!specificRangeHit.Contains(hit.transform)) { specificRangeHit.Add(hit.transform); }

                if (specificRangeHit.Count > 1 && detectObj != null)
                {
                    foreach (Transform obj in specificRangeHit)
                    {
                        float detectObjDist = Vector3.Distance(rayStart, detectObj.transform.position);
                        float hitDist = Vector3.Distance(rayStart, hitObj.transform.position);

                        if (hitDist < detectObjDist)
                        {
                            // Debug.Log("change");
                            detectObj = obj.gameObject;
                        }

                    }
                }
                else
                {
                    //Debug.Log("new");
                    detectObj = hit.transform.gameObject;
                }
            }
            else
            {
                if(specificRangeHit.Contains(hit.transform))
                {
                    specificRangeHit.Remove(hit.transform);
                }
                detectObj = null;
            }
        }

        if(uiManager.gameObject.activeInHierarchy)
        {
            if (detectObj != null)
            {
                uiManager.activeObjectName.text = detectObj.name;
                
                if(!uiManager.interacting)
                {
                    uiManager.activeObjectButton.SetActive(true);
                }
                else
                {
                    uiManager.activeObjectButton.SetActive(false);
                }
            }
            else
            {
                uiManager.activeObjectButton.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(camera_First.transform.position, sphereRadius);
    }

    private void Player_InteractObject()
    {
        if (player_Body.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                /*if (!isUse || !isClear)
                {
                    // Continue interact
                }*/
            }
        }
    }

    private void Player_Die()
    {
        if (playerLife <= 0)
        {
            this.player_Body.SetActive(false);
            this.player_Body.layer = LayerMask.NameToLayer("Default"); // Default Layer
            this.GetComponent<Collider>().enabled = false;
            pv.RPC("RPC_PlayerDie", RpcTarget.Others);
        }
    }

    [PunRPC]
    void RPC_PlayerDie()
    {
        this.player_Body.SetActive(false);
        this.player_Body.layer = LayerMask.NameToLayer("Default"); // Default Layer
        this.GetComponent<Collider>().enabled = false;
    }

    private void Camera_Change()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (camera_First.activeSelf == true)
            {
                camera_First.SetActive(false);
                camera_Third.SetActive(true);
            }
            else
            {
                camera_First.SetActive(true);
                camera_Third.SetActive(false);
            }
        }
    }

    private void Camera_Rotate()
    {
        //rot_X = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -5f, 5f); // Up and down
        rot_Y = Input.GetAxis("Mouse X"); // Left and right

        Debug.Log(rot_X);

        if (rot_Y != 0)
        {
            camera_Rotation.localEulerAngles += new Vector3(0, rot_Y, 0) * speed_Rotate;
        }
        //if (rot_X != 0)
        //{
        //    camera_Rotation.localEulerAngles = new Vector3(rot_X, 0, 0) * speed_Rotate;
        //}
    }

    /* -------------------------------------------------- */

    // 사운드
    IEnumerator PlaySound(string state)
    {
        if(state == "Walk")
        {
            if(playerAudioSource.clip != walkSound)
            {
                playerAudioSource.Stop();
                playerAudioSource.clip = null;
                playerAudioSource.clip = walkSound;
                playerAudioSource.Play();
            }
        }
        else if(state == "Run")
        {
            if (playerAudioSource.clip != runSound)
            {
                playerAudioSource.Stop();
                playerAudioSource.clip = null;
                playerAudioSource.clip = runSound;
                playerAudioSource.Play();
            }
        }

        yield return new WaitForSeconds(1);
    }
}