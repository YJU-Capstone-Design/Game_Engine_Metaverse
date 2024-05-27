using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Character_Controller : MonoBehaviour
{
    /* -------------------------------------------------- */

    [Header("Component")]
    [SerializeField] private Animator animator;
    [SerializeField] private PhotonManager photonManager;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private PhotonTransformView photonTransformView;

    [Header("Parts")]
    // Insert "Body" Object
    public GameObject player_Body;
    // Camera (1st or 3rd)
    public GameObject camera_First, camera_Third;
    // Insert "Rotate_Horizontal" Object
    public Transform camera_Rotation;

    [Header("Speed")]
    public float speed_Walk = 10f;
    public float speed_Run = 15f;
    public float speed_Rotate = 1f;

    // Concealed variable
    // Player Position
    private float pos_X, pos_Z;
    // Camera Rotation
    private float rot_X, rot_Y;

    /* -------------------------------------------------- */

    private void Awake()
    {
        Player_Init();
    }

    private void Player_Init()
    {
        photonManager = LobbyUIManager.Instance.photonManager;
        photonView = GetComponent<PhotonView>();
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

    private void OnEnable()
    {
        if (!LobbyUIManager.Instance.photonManager.playerList.Contains(this.gameObject))
        {
            photonView.RPC("AddMeToList", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void AddMeToLIst()
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
    }

    private void Player_Setting()
    {
        animator = GetComponent<Animator>();

        if (photonView.IsMine)
        {
            this.gameObject.name += "(Local Player)";
        }
        else
        {
            this.gameObject.name += "(Other Player)";
        }
    }

    private void Camera_Setting()
    {
        if (photonView.IsMine)
        {
            // Enable and setting local player's third camera 
            camera_First.SetActive(false);
            camera_Third.SetActive(true);
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
            Player_Move();

            // Camera code
            Camera_Move();
            Camera_Change();
        }
    }

    private void Player_Move()
    {
        pos_X = Input.GetAxis("Horizontal");
        pos_Z = Input.GetAxis("Vertical");

        if (pos_X != 0 || pos_Z != 0) // isMove
        {
            // Direction vector of the camera
            Vector3 moveDir_Vector3 = (camera_Rotation.localRotation * Vector3.forward).normalized;
            Vector3 moveDir_Forward = moveDir_Vector3 * pos_Z;
            Vector3 moveDir_Right = Quaternion.Euler(0, 90, 0) * moveDir_Vector3 * pos_X;

            // Player move direction
            Vector3 moveDir = moveDir_Forward + moveDir_Right;

            // Player rotate
            player_Body.transform.localRotation = Quaternion.Slerp(player_Body.transform.rotation, Quaternion.LookRotation(moveDir.normalized), speed_Rotate);

            animator.SetBool("Walk", true);

            if (Input.GetKey(KeyCode.LeftShift)) // isRun
            {
                // Run State
                animator.SetBool("Run", true);
                transform.Translate(moveDir.normalized * speed_Run * Time.deltaTime);
            }
            else // !isRun
            {
                // Walk State
                animator.SetBool("Run", false);
                transform.Translate(moveDir.normalized * speed_Walk * Time.deltaTime);
            }
        }
        else // !isMove
        {
            // Idle State
            animator.SetBool("Walk", false);
        }
    }

    private void Camera_Move()
    {
        rot_X = Input.GetAxis("Mouse Y"); // Up and down
        rot_Y = Input.GetAxis("Mouse X"); // Left and right

        if (rot_Y != 0)
        {
            camera_Rotation.localEulerAngles += new Vector3(0, rot_Y, 0) * speed_Rotate;
        }
    }

    private void Camera_Change()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (camera_Third.activeSelf == true)
            {
                camera_First.SetActive(true);
                camera_Third.SetActive(false);
            }
            else
            {
                camera_First.SetActive(false);
                camera_Third.SetActive(true);
            }
        }
    }
}
