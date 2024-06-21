using Photon.Pun;
using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public PhotonManager photonManager;
    public PhotonView pv;
    public AudioManager audioManager;

    [Header("# UI Boxs")]
    [SerializeField] List<GameObject> activeUIChildren;
    bool isCheckAnswer = false;

    [Header("# Active Objects")]
    [SerializeField] List<GameObject> doors;
    [SerializeField] List<GameObject> activeObjects; // ��� �� ��ȣ�ۿ��� �Ұ����ϰ� ���� ������Ʈ�� (ex. �ڹ���)
    [SerializeField] List<GameObject> hintObjects; // ��Ʈ ��� �� ��ȣ�ۿ��ϴ� ������Ʈ��
    [SerializeField] List<GameObject> hintButtons; // ��Ʈ ��ư��
    bool getSchoolsupplies = false;
    bool breakLock = false;
    [SerializeField] GameObject[] particles; // ��Ʈ ��� �� Ȱ��ȭ �� ��ƼŬ

    [Header("# Player Info")]
    [SerializeField] TextMeshProUGUI timerText;
    public float playTime;
    public GameObject activeObjectButton;
    public TextMeshProUGUI activeObjectName;
    public bool interacting = false; // ���� ��ȣ�ۿ� ������ ����
    public GameObject[] playerLife; // �÷��̾� ����(��Ʈ) UI

    [Header("# Direction Lock")]
    public List<string> dirLockInput;
    string[] dirLockAnswer; // ���� �ڹ��� ����
    bool checkDirectioin = false;

    [Header("# Button Lock")]
    public List<int> btnLockInput;
    int[] btnLockAnswer; // ��ư �ڹ��� ����
    [SerializeField] RectTransform btnLockCheckButton; // Ȯ�� ��ư -> �ִϸ��̼� �뵵
    [SerializeField] ButtonLock[] btnLockButtons; // Ŭ�� ������ ��ư��

    [Header("# Dial Lock")]
    public List<int> dialLockInput;
    int[] dialLockAnswer;
    [SerializeField] DialLock[] dialLockTexts; // Dial Lock ���� ĭ

    [Header("# Key Lock")]
    [SerializeField] TextMeshProUGUI fluidKeyText; // "28 + ���� ��" ���� ������ Ű
    bool getKey = false;

    [Header("# TV / Remote")]
    public List<string> tvInput;
    string tvAnswer;
    public GameObject tvInputField;
    public TextMeshProUGUI tvInputText;
    bool getUSB = false;
    public bool connetUSB = false;
    public bool tvPowerOn = false;

    [Header("# DoorLock")]
    public List<string> doorLockInput;
    string doorLockAnswer;
    public TextMeshProUGUI doorLockInputText;

    [Header("# Question Button")]
    [SerializeField] GameObject[] answerBtns;
    [SerializeField] Sprite[] btnSprites;

    [Header("# Wallet")]
    [SerializeField] GameObject[] walletObjs;

    [Header("# White Board")]
    [SerializeField] GameObject clueNote;
    [SerializeField] GameObject[] victimClueNotes;

    [Header("# Refrigerator")]
    [SerializeField] GameObject backGroundImg;

    [Header("# Narration")]
    Narration narration;
    public GameObject narrationBox;
    [SerializeField] TextMeshProUGUI narrationText;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        narration = GetComponent<Narration>();

        InGameSetting();

        DirectionLockSetting();
        ButtonLockSetting();
        DialLockSetting();
        TVSetting();
        DoorLockSetting();
        KeyLockSetting();
    }

    private void OnEnable()
    {
        InGameSetting();

        DirectionLockSetting();
        ButtonLockSetting();
        DialLockSetting();
        TVSetting();
        DoorLockSetting();
        KeyLockSetting();

        foreach (GameObject activeObj in activeObjects)
        {
            activeObj.SetActive(true);
            activeObj.GetComponent<Collider>().enabled = true;
        }

        doors[0].GetComponent<Animator>().SetBool("open", false);
        doors[1].GetComponent<opencloseDoor1>().openandclose1.Play("Closing 1");
        doors[1].gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        doors[1].GetComponent<opencloseDoor1>().open = false;

        // ��ƼŬ ��Ȱ��ȭ
        particles[0].SetActive(false);
        particles[1].SetActive(false);
        particles[2].SetActive(false);
    }

    private void Update()
    {
        // ���� �ڹ��� �Է� ���� 12�� �� ��� ���� Ȯ��
        if (dirLockInput.Count == 12 && !checkDirectioin)
        {
            Debug.Log(checkDirectioin);
            checkDirectioin = true;
            CheckLockAnswer("Direction");
        }

        // �����ִ� ������Ʈ ����
        if (Input.GetKeyDown(KeyCode.Escape) && !isCheckAnswer)
        {
            // ȭ��Ʈ����
            if (clueNote.activeInHierarchy)
            {
                clueNote.SetActive(false);

                foreach (GameObject clueNote in victimClueNotes)
                {
                    clueNote.SetActive(false);
                }
            }
            else if (narrationBox.activeInHierarchy && activeUIChildren[19].activeInHierarchy) // ����� ����ĭ
            {
                narrationBox.SetActive(false);
                backGroundImg.SetActive(false);
            }
            else
            {
                for (int i = 0; i < activeUIChildren.Count; i++)
                {
                    if (activeUIChildren[i].activeInHierarchy)
                    {
                        CloseAllUI();
                        break;
                    }

                    // ������ ��ȣ
                    if (activeUIChildren.Count - 1 == i)
                    {
                        photonManager.SettingBtn();
                    }
                }
            }

            // SFX Sound
            audioManager.SFX(0);
            Debug.Log("Setting Button");
        }

        // ��ȣ�ۿ� ������ ������Ʈ ��ư�� Ȱ��ȭ �Ǿ��� ��
        if (activeObjectButton.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                OpenNarration(activeObjectName.text);

                // SFX Sound
                audioManager.SFX(0);
                Debug.Log("Setting Button");
            }
        }

        // narrationBox �� Ȱ��ȭ �Ǿ��� ��, Enter Ű�� ������ ����� ���� ��쿣 ��� �۵�, �ƴϸ� narrationBox ��Ȱ��ȭ
        if (narrationBox.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (narrationText.text == narration.directionLock)
                {
                    activeUIChildren[7].SetActive(true); // ���� �ڹ��� UI Ȱ��ȭ
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.buttonLock)
                {
                    activeUIChildren[10].SetActive(true); // ��ư �ڹ��� UI Ȱ��ȭ
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.keyLock_2)
                {
                    activeUIChildren[12].SetActive(true); // ���� �ڹ��� UI Ȱ��ȭ
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.deadBodyBag)
                {
                    activeUIChildren[5].SetActive(true); // ���� UI Ȱ��ȭ
                    OpenNarration("Wallet");
                }
                else if (narrationText.text == narration.wallet)
                {
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.hint)
                {
                    if (photonManager.hintCount > 0)
                    {
                        pv.RPC("UseHint", RpcTarget.All);
                    }
                    interacting = false;
                    CloseAllUI();
                }
                else if (narrationText.text == narration.livingroomTV_2)
                {
                    connetUSB = true;
                    activeUIChildren[16].SetActive(true); // TV/Remote UI Ȱ��ȭ
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.doorLock)
                {
                    activeUIChildren[17].SetActive(true); // DoorLock UI Ȱ��ȭ
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.whiteBoard)
                {
                    activeUIChildren[18].SetActive(true); // WhiteBoard UI Ȱ��ȭ
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.refrigerator && !activeObjects[5].activeInHierarchy)
                {
                    activeUIChildren[19].SetActive(true); // ����� UI Ȱ��ȭ
                    narrationBox.SetActive(false);
                }
                else
                {
                    interacting = false;

                    CloseAllUI();
                }

                // SFX Sound
                audioManager.SFX(0);
                Debug.Log("Setting Button");
            }
            else if (narrationText.text == narration.refrigerator)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                {
                    activeUIChildren[11].SetActive(true); // ��ȣ �ڹ��� UI Ȱ��ȭ
                    narrationBox.SetActive(false);
                    pv.RPC("UsingLock", RpcTarget.All, "Button", true);

                    // SFX Sound
                    audioManager.SFX(0);
                    Debug.Log("Setting Button");
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                {
                    activeUIChildren[19].SetActive(true); // ����� UI Ȱ��ȭ
                    narrationBox.SetActive(false);
                    pv.RPC("UsingLock", RpcTarget.All, "Button", true);

                    // SFX Sound
                    audioManager.SFX(0);
                    Debug.Log("Setting Button");
                }
            }
        }
    }

    private void Start()
    {
        // Player List ����
        photonManager.SetPlayerList();

        // fluidKeyText ����
        fluidKeyText.text = (28 + photonManager.playerList.Count).ToString();
    }

    public void InGameSetting()
    {
        playTime = 900; // 15min

        // bool �� �ʱ�ȭ
        getKey = false;
        isCheckAnswer = false;
        interacting = false;
        checkDirectioin = false;
        getUSB = false;
        connetUSB = false;
        tvPowerOn = false;
        breakLock = false;
        getSchoolsupplies = false;

        foreach (GameObject life in playerLife)
        {
            life.SetActive(true);
        }

        foreach (GameObject activeObj in activeObjects)
        {
            activeObj.GetComponent<Collider>().enabled = true;
        }
    }

    // Ÿ�̸� �Լ��� PhotonManager ���� ����
    [PunRPC]
    void Timer(float time)
    {
        if (time > 0)
        {
            int minute = (int)(time / 60);
            int second = (int)(time % 60);

            timerText.text = $"{minute} : {second}";
        }
        else
        {
            time = 0;
            timerText.text = "00 : 00";

            // ���� UI Ȱ��ȭ
        }
    }

    void OpenNarration(string objName)
    {
        // narrationBox Ȱ��ȭ
        narrationBox.SetActive(true);
        activeUIChildren[0].SetActive(true);

        interacting = true;

        switch (objName)
        {
            case "Bed":
                narrationText.text = narration.bed;
                break;
            case "DeadBody":
                narrationText.text = narration.deadBody;
                break;
            case "Chair":
                narrationText.text = narration.chair;
                break;
            case "Laptop":
                narrationText.text = narration.laptop;
                activeUIChildren[8].SetActive(true);
                break;
            case "Document":
                narrationText.text = narration.document;
                break;
            case "PlayerBag":
                narrationText.text = narration.playerBag;
                pv.RPC("GetSchoolsupplies", RpcTarget.All);
                break;
            case "DeadBodyBag":
                narrationText.text = narration.deadBodyBag;
                break;
            case "Wallet":
                narrationText.text = narration.wallet;
                break;
            case "IDCard":
                narrationText.text = narration.IDcard;
                break;
            case "WallClock":
                narrationText.text = narration.wallClock;
                break;
            case "KitchenKnife":
                narrationText.text = narration.kitchenKnife;
                break;
            case "WallTV":
                narrationText.text = narration.WallTV;
                break;
            case "DirectionLock":
                narrationText.text = narration.directionLock;
                pv.RPC("UsingLock", RpcTarget.All, "Direction", true);
                break;
            case "ButtonLock":
                narrationText.text = narration.buttonLock;
                pv.RPC("UsingLock", RpcTarget.All, "Button", true);
                break;
            case "DialLock":
                narrationText.text = narration.dialLock;
                pv.RPC("UsingLock", RpcTarget.All, "Dial", true);
                break;
            case "KeyLock":
                if (!getKey)
                {
                    narrationText.text = narration.keyLock_1;
                }
                else if (getKey)
                {
                    narrationText.text = narration.keyLock_2;
                    pv.RPC("UsingLock", RpcTarget.All, "KeyLock", true);
                }
                break;
            case "GetKey":
                narrationText.text = narration.key;
                break;
            case "StorageCloset":
                narrationText.text = narration.storageCloset;
                break;
            case "Hint":
                narrationText.text = narration.hint;
                break;
            case "HintZero":
                narrationText.text = narration.hintZero;
                break;
            case "Remote":
                narrationText.text = narration.remote;
                break;
            case "GetUSB":
                narrationText.text = narration.getUSB;
                break;
            case "LivingRoomTV":
                if (!getUSB)
                {
                    narrationText.text = narration.livingroomTV_1;
                }
                else if (getUSB)
                {
                    narrationText.text = narration.livingroomTV_2;
                    pv.RPC("UsingLock", RpcTarget.All, "TV", true);
                }
                break;
            case "UsbInfo":
                narrationText.text = narration.usbInfo;
                break;
            case "WhiteBoard":
                narrationText.text = narration.whiteBoard;
                break;
            case "ExitDoor":
                narrationText.text = narration.doorLock;
                pv.RPC("UsingLock", RpcTarget.All, "DoorLock", true);
                break;
            case "Refrigerator":
                narrationText.text = narration.refrigerator;
                pv.RPC("UsingLock", RpcTarget.All, "Dial", true);
                break;
            case "Refrigerator_1":
                narrationText.text = narration.refrigerator_1;
                break;
            case "Refrigerator_2":
                narrationText.text = narration.refrigerator_2;
                break;
        }
    }

    void DirectionLockSetting()
    {
        // ���� �ڹ��� �ʱ�ȭ
        dirLockInput = new List<string>();

        // ���� �ڹ��� ���� ���� ��7, ��3, ��2 �� �����ؾ� ��.
        dirLockAnswer = new string[12] { "Up", "Up", "Up", "Up", "Up", "Up", "Up", "Right", "Right", "Right", "Down", "Down" };

        checkDirectioin = false;
    }

    void ButtonLockSetting()
    {
        btnLockInput = new List<int>();

        btnLockAnswer = new int[4] { 1, 2, 3, 6 };
    }

    void DialLockSetting()
    {
        dialLockInput = new List<int> { 0, 0, 0 };

        dialLockAnswer = new int[3] { 5, 2, 5 };
    }

    void TVSetting()
    {
        tvInput = new List<string>();
        tvAnswer = "12";
        tvInputText.text = "";
    }

    void DoorLockSetting()
    {
        doorLockInput = new List<string>();
        doorLockAnswer = "0325";
        doorLockInputText.text = "";
    }

    void KeyLockSetting()
    {
        getKey = false;
    }

    // �ڹ��� ���� Ȯ��
    public void CheckLockAnswer(string name)
    {
        Debug.Log("Check Lock");

        // ���� �ڹ���
        if (name == "Direction")
        {
            for (int i = 0; i < dirLockInput.Count; i++)
            {
                if (dirLockInput[i] != dirLockAnswer[i] || dirLockInput[i] == null)
                {
                    Debug.Log("����");
                    dirLockInput.Clear(); // �Է� �� �ʱ�ȭ

                    // ���� ����
                    StartCoroutine(FailLock());
                    break;
                }
                else if (dirLockInput[dirLockInput.Count - 1] == dirLockAnswer[dirLockInput.Count - 1])
                {
                    Debug.Log("����");

                    // ���� ����
                    StartCoroutine(SuccessLock("Direction"));
                    break;
                }
            }
        }
        else if (name == "Button")
        {
            if (btnLockInput.Count > btnLockAnswer.Length || btnLockInput.Count < btnLockAnswer.Length)
            {
                Debug.Log("����");
                btnLockInput.Clear(); // �Է� �� �ʱ�ȭ

                // ���� ����
                StartCoroutine(FailLock());
            }
            else if (btnLockInput.Count == btnLockAnswer.Length)
            {
                btnLockInput.Sort(); // ���� ������ ����

                for (int i = 0; i < btnLockInput.Count; i++)
                {
                    if (btnLockInput[i] != btnLockAnswer[i])
                    {
                        Debug.Log("����");
                        btnLockInput.Clear(); // �Է� �� �ʱ�ȭ

                        // ���� ����
                        StartCoroutine(FailLock());

                        break;
                    }
                    else if (btnLockInput[btnLockInput.Count - 1] == btnLockAnswer[btnLockInput.Count - 1])
                    {
                        Debug.Log("����");

                        // ���� ����
                        StartCoroutine(SuccessLock("Button"));
                    }
                }
            }

            // ��ư �ִϸ��̼�
            CheckButtonAnim();

            // ��ư �ʱ�ȭ
            foreach (ButtonLock btn in btnLockButtons)
            {
                btn.upButton.SetActive(true);
                btn.downButton.SetActive(false);
            }
        }
        else if (name == "Dial")
        {
            for (int i = 0; i < dialLockAnswer.Length; i++)
            {
                if (dialLockInput[i] != dialLockAnswer[i])
                {
                    Debug.Log("����");

                    // ���� ����
                    StartCoroutine(FailLock());

                    break;
                }

                if (i == 2 && dialLockInput[dialLockInput.Count - 1] == dialLockAnswer[dialLockInput.Count - 1])
                {
                    Debug.Log("����");

                    // ���� ����
                    StartCoroutine(SuccessLock("Dial"));
                }
            }
        }
        else if (name == "TV")
        {
            if (tvInputText.text == tvAnswer)
            {
                Debug.Log("����");

                // ���� ����
                StartCoroutine(SuccessLock("TV"));
            }
            else
            {
                Debug.Log("����");
                TVSetting();

                // ���� ����
                StartCoroutine(FailLock());
            }
        }
        else if (name == "DoorLock")
        {
            if (doorLockInputText.text == doorLockAnswer)
            {
                Debug.Log("����");

                StartCoroutine(SuccessLock("DoorLock"));
            }
            else
            {
                Debug.Log("����");
                DoorLockSetting();

                // ���� ����
                StartCoroutine(FailLock());
            }
        }
    }


    // ��� �ڹ��� �� �ʱ�ȭ �Լ�
    void ResetInput(string lockName)
    {
        switch (lockName)
        {
            case "Direction":
                dirLockInput.Clear(); // �Է� �� �ʱ�ȭ
                checkDirectioin = false;
                break;
            case "Button":
                btnLockInput.Clear();

                // ��ư �ʱ�ȭ
                foreach (ButtonLock btn in btnLockButtons)
                {
                    btn.upButton.SetActive(true);
                    btn.downButton.SetActive(false);
                }

                break;
            case "Dial":
                DialLockSetting(); // List �� 0,0,0 ���� �״�� �־�� �ϱ⿡ Clear �� ����ϸ� �ȵ�.

                foreach (DialLock num in dialLockTexts)
                {
                    num.SetInitialValue();
                }

                break;
            case "TV":
                TVSetting();
                break;
            case "DoorLock":
                DoorLockSetting();
                break;
        }
    }

    // �ڹ��� Reset ��ư
    public void ResetInputBtn(string name)
    {
        ResetInput(name);

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Reset Lock Button");
    }

    // ��ư �ڹ��� Ȯ�� ��ư �ִϸ��̼�
    public void CheckButtonAnim()
    {
        // Anchor �ʱ�ȭ
        Vector2 originalAnchorMin = new Vector2(0.4f, -0.05f);
        Vector2 originalAnchorMax = new Vector2(0.55f, 0);
        Vector2 nextAnchorMin = new Vector2(0.5f, -0.05f);
        Vector2 nextAnchorMax = new Vector2(0.65f, 0);

        StartCoroutine(SmoothCoroutine(btnLockCheckButton, originalAnchorMin, originalAnchorMax, nextAnchorMin, nextAnchorMax, 0.1f));
    }

    // ���� �ڹ��� Key ��ư
    public void UseKeyButton(bool answer)
    {
        if (answer)
        {
            Debug.Log("���� �ڹ��� ����");

            KeyLockSetting();

            // ���� ����
            StartCoroutine(SuccessLock("Key"));
        }
        else
        {
            Debug.Log("���� �ڹ��� ����");

            // ���� ����
            StartCoroutine(FailLock());
        }
    }

    // ���� �ذ� �� ����Ǵ� �Լ�
    IEnumerator SuccessLock(string name)
    {
        activeUIChildren[13].SetActive(true);

        isCheckAnswer = true;

        switch (name)
        {
            case "Direction":
            case "Button":
            case "Dial":
            case "TV":
            case "Key":
                // SFX Sound
                audioManager.SFX(5);
                break;
        }

        yield return new WaitForSeconds(1);

        isCheckAnswer = false;

        // UI ��Ȱ��ȭ
        CloseAllUI();

        interacting = false;

        // �ʱ�ȭ
        switch (name)
        {
            case "Direction":
                DirectionLockSetting();
                // SFX Sound
                audioManager.SFX(16);
                OpenNarration("GetKey");
                break;
            case "Button":
                ButtonLockSetting();
                // SFX Sound
                audioManager.SFX(16);
                OpenNarration("GetUSB");
                break;
            case "Dial":
                DialLockSetting();
                break;
            case "TV":
                TVSetting();
                OpenNarration("UsbInfo");
                break;
            case "DoorLock":
                DoorLockSetting();
                break;
        }

        yield return new WaitForSeconds(0.2f);

        // ���� ��� (����)
        pv.RPC("OpenDoor", RpcTarget.All, name);
    }

    // ���� �ذ� ���� �� ����Ǵ� �Լ�
    IEnumerator FailLock()
    {
        activeUIChildren[14].SetActive(true);

        isCheckAnswer = true;

        // SFX Sound
        audioManager.SFX(2);

        yield return new WaitForSeconds(1);

        // ���� �ڹ���
        checkDirectioin = false;

        isCheckAnswer = false;

        // ���� UI ��Ȱ��ȭ
        activeUIChildren[14].SetActive(false);

        // ���� �� ���ѽð� 30�� ����
        pv.RPC("ReduceTime", RpcTarget.All);
    }

    public void BreakLockButton(string name)
    {
        StartCoroutine(SuccessLock(name));

        pv.RPC("BreakLock", RpcTarget.All);

        // SFX Sound
        audioManager.SFX(0);
    }

    [PunRPC]
    public void BreakLock()
    {
        breakLock = true;

        foreach (GameObject hintButton in hintButtons)
        {
            hintButton.SetActive(false);
        }
    }

    [PunRPC]
    void OpenDoor(string name)
    {
        Animator doorAnim;

        switch (name)
        {
            case "Direction":
                activeObjects[0].GetComponent<Collider>().enabled = false;
                getKey = true;
                break;
            case "Button":
                activeObjects[1].GetComponent<Collider>().enabled = false;
                // TV Ű�е� Ȱ��ȭ
                getUSB = true;
                break;
            case "Dial":
                activeObjects[6].SetActive(false); // ���� ���� ��Ȱ��ȭ
                narration.refrigerator = "����� �Ʒ� ĭ�� ���� ����ִ� �� ����.\n�Ʒ� ĭ�� ���캸�� ���Ͻø� Enter�� �����ֽʽÿ�...";

                // SFX Sound
                audioManager.SFX(14);
                break;
            case "Key":
                activeObjects[7].GetComponent<Collider>().enabled = false;

                doorAnim = doors[0].GetComponent<Animator>();
                doorAnim.SetBool("open", true);
                // SFX Sound
                audioManager.SFX(6);
                break;
            case "TV":
                activeObjects[5].SetActive(false); // �ֹ� ���� ��Ȱ��ȭ
                break;
            case "DoorLock":
                // ������ ������
                doorAnim = doors[1].GetComponent<Animator>();
                doorAnim.Play("Opening 1");
                activeObjects[4].GetComponent<Collider>().enabled = false;

                // SFX Sound
                audioManager.SFX(7);
                break;
        }
    }

    [PunRPC]
    void ReduceTime()
    {
        playTime -= 30;
    }

    [PunRPC]
    void UsingLock(string name, bool usingLock)
    {
        if (usingLock)
        {
            switch (name)
            {
                case "Direction":
                    activeObjects[0].layer = 0;
                    break;
                case "Button":
                    activeObjects[1].layer = 0;
                    break;
                case "Dial":
                    activeObjects[3].layer = 0;
                    break;
                case "TV":
                    activeObjects[2].layer = 0;
                    break;
                case "DoorLock":
                    activeObjects[4].layer = 0;
                    break;
                case "KeyLock":
                    activeObjects[7].layer = 0;
                    break;
            }
        }
        else
        {
            switch (name)
            {
                case "Direction":
                    activeObjects[0].layer = 6;
                    break;
                case "Button":
                    activeObjects[1].layer = 6;
                    break;
                case "Dial":
                    activeObjects[3].layer = 6;
                    break;
                case "TV":
                    activeObjects[2].layer = 6;
                    break;
                case "DoorLock":
                    activeObjects[4].layer = 6;
                    break;
                case "KeyLock":
                    activeObjects[7].layer = 6;
                    break;
            }
        }

    }

    // ȭ��Ʈ���� ClueNote ��ư
    public void CheckClueNote(string name)
    {
        clueNote.SetActive(true);

        switch (name)
        {
            case "Lee":
                victimClueNotes[0].SetActive(true);
                break;
            case "Armstrong":
                victimClueNotes[1].SetActive(true);
                break;
            case "SamSu":
                victimClueNotes[2].SetActive(true);
                break;
            case "SooYoung":
                victimClueNotes[3].SetActive(true);
                break;
            case "Minji":
                victimClueNotes[4].SetActive(true);
                break;
        }

        // SFX Sound
        audioManager.SFX(0);
    }

    // ����� ���� ĭ ��ư
    public void CheckRefrigerator(int num)
    {
        backGroundImg.SetActive(true);

        switch (num)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                OpenNarration("Refrigerator_1");
                break;
            case 7:
                OpenNarration("Refrigerator_2");
                break;

        }

        // SFX Sound
        audioManager.SFX(14);
    }

    // ��Ʈ ���
    [PunRPC]
    void UseHint()
    {
        if (photonManager.hintCount <= 0)
            return;

        // ���� Ƚ���� 1ȸ�� ��, �ٸ� ��� UI �� ����
        //if (photonManager.hintCount == 1)
        //{
        //    narrationBox.SetActive(false);
        //    activeUIChildren[0].SetActive(false);
        //}

        // ��Ʈ ��� ���� �ʿ�
        if (photonManager.hintCount == 2)
        {
            // ����
            hintObjects[0].layer = 6;

            // ��ƼŬ ������Ʈ Ȱ��ȭ �ʿ�
            particles[0].SetActive(true);
            particles[1].SetActive(true);
        }
        else if (photonManager.hintCount == 1)
        {
            // �÷��̾� ����
            hintObjects[1].layer = 6;

            // ��ƼŬ ������Ʈ Ȱ��ȭ �ʿ�
            particles[2].SetActive(true);
        }

        photonManager.hintCount--;
    }

    [PunRPC]
    void GetSchoolsupplies()
    {
        getSchoolsupplies = true;

        foreach (GameObject hintButton in hintButtons)
        {
            hintButton.SetActive(true); ;
        }

        particles[2].SetActive(false); // ��ƼŬ ��Ȱ��ȭ

        // �÷��̾� ����
        hintObjects[1].layer = 0;

    }



    // �ΰ��� ��Ʈ ��ư
    public void HintButton()
    {
        interacting = true;

        if (photonManager.hintCount > 0)
        {
            OpenNarration("Hint");
        }
        else
        {
            OpenNarration("HintZero");
        }

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Hint Button Sound");
    }

    // Active UI �� ���� ��, �ʱ�ȭ�� �ʿ��� ������Ʈ�� �ʱ�ȭ
    void CloseAcvtiveUI(GameObject obj)
    {
        if (obj.name.Contains("Question"))
        {
            foreach (GameObject btn in answerBtns)
            {
                Image btnImage = btn.GetComponent<Image>();
                TextMeshProUGUI text = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                btnImage.sprite = btnSprites[0];
                text.color = Color.white;
            }
        }
        else if (obj.name.Contains("Lock"))
        {
            // ��� �ڹ��� �ʱ�ȭ
            ResetInput("Direction");
            ResetInput("Button");
            ResetInput("Dial");
            ResetInput("TV");
            ResetInput("DoorLock");
        }
        else if (obj.name.Contains("Wallet"))
        {
            for (int i = 0; i < 3; i++)
            {
                Animator animWallet = obj.transform.GetChild(i).GetComponent<Animator>();
                animWallet.SetBool("Click", false);
            }

            obj.transform.GetChild(1).GetComponent<Button>().enabled = true; // �ź��� ��ư ��� Ȱ��ȭ 
        }
    }

    // ���� ��ư Ŭ�� �̺�Ʈ
    public void ClickAnswer(int index)
    {
        for (int i = 0; i < answerBtns.Length; i++)
        {
            Image btnImage;
            TextMeshProUGUI text;

            if (i == index)
            {
                btnImage = answerBtns[index].GetComponent<Image>();
                text = answerBtns[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                btnImage.sprite = btnSprites[1];
                text.color = Color.black;
            }
            else
            {
                btnImage = answerBtns[i].GetComponent<Image>();
                text = answerBtns[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                btnImage.sprite = btnSprites[0];
                text.color = Color.white;
            }
        }
    }

    // ���� �ִϸ��̼�
    public void CheckIDCard(GameObject obj)
    {
        Animator anim = obj.GetComponent<Animator>();

        if (anim != null && !narrationBox.activeInHierarchy)
        {
            foreach (GameObject wallet in walletObjs)
            {
                anim = wallet.GetComponent<Animator>();
                anim.SetBool("Click", true);
            }

            obj.GetComponent<Button>().enabled = false; // ��ư ����� ��Ȱ��ȭ 
        }

        // SFX Sound
        audioManager.SFX(0);
    }

    // ��� UI ���� ��ư
    public void CloseAllUI()
    {
        foreach (GameObject obj in activeUIChildren)
        {
            if (obj.activeInHierarchy) 
            {
                if(obj.name.Contains("Narration"))
                {
                    Debug.Log("Narration");
                    if (narrationText.text == narration.directionLock)
                    {
                        pv.RPC("UsingLock", RpcTarget.All, "Direction", false);
                        Debug.Log("Dddd");
                    }
                    else if (narrationText.text == narration.buttonLock)
                    {
                        pv.RPC("UsingLock", RpcTarget.All, "Button", false);
                    }
                    else if (narrationText.text == narration.refrigerator)
                    {
                        pv.RPC("UsingLock", RpcTarget.All, "Dial", false);
                    }
                    else if (narrationText.text == narration.keyLock_2)
                    {
                        pv.RPC("UsingLock", RpcTarget.All, "KeyLock", false);
                    }
                    else if (narrationText.text == narration.livingroomTV_2)
                    {
                        pv.RPC("UsingLock", RpcTarget.All, "TV", false);
                    }
                    else if (narrationText.text == narration.doorLock)
                    {
                        pv.RPC("UsingLock", RpcTarget.All, "DoorLock", false);
                    }
                }
                else if (obj.name.Contains("Direction"))
                {
                    pv.RPC("UsingLock", RpcTarget.All, "Direction", false);
                    Debug.Log("dsfsdfffefewfewf");
                }
                else if (obj == activeUIChildren[10])
                {
                    pv.RPC("UsingLock", RpcTarget.All, "Button", false);
                }
                else if (obj == activeUIChildren[11])
                {
                    pv.RPC("UsingLock", RpcTarget.All, "Dial", false);
                }
                else if (obj == activeUIChildren[12])
                {
                    pv.RPC("UsingLock", RpcTarget.All, "KeyLock", false);
                }
                else if (obj == activeUIChildren[16])
                {
                    pv.RPC("UsingLock", RpcTarget.All, "TV", false);
                }
                else if (obj == activeUIChildren[17])
                {
                    pv.RPC("UsingLock", RpcTarget.All, "DoorLock", false);
                }

                CloseAcvtiveUI(obj);
                obj.SetActive(false);

            }

            activeUIChildren[5].transform.GetChild(1).GetComponent<Button>().enabled = true; // ���� ��ư ��� Ȱ��ȭ
        }

        interacting = false;

        narrationText.text = "";

        //for (int i = 0; i < 5; i++)
        //{
        //    activeObjects[i].layer = 6;
        //}
        //activeObjects[7].layer = 6;
    }

    IEnumerator SmoothCoroutine(RectTransform target, Vector2 currentMin, Vector2 currentMax, Vector2 nextMin, Vector2 nextMax, float time)
    {
        Vector3 velocity = Vector3.zero;

        target.anchorMin = currentMin;
        target.anchorMax = currentMax;

        float offset = 0.01f;

        while (nextMin.x - offset >= target.anchorMin.x && nextMax.x - offset >= target.anchorMax.x)
        {
            target.anchorMin
                = Vector3.SmoothDamp(target.anchorMin, nextMin, ref velocity, time);

            target.anchorMax
                = Vector3.SmoothDamp(target.anchorMax, nextMax, ref velocity, time);

            yield return null;
        }

        target.anchorMin = nextMin;
        target.anchorMax = nextMax;

        yield return new WaitForSeconds(0.1f);

        while (nextMin.x + offset <= target.anchorMin.x && nextMax.x + offset <= target.anchorMax.x)
        {
            target.anchorMin
                = Vector3.SmoothDamp(target.anchorMin, nextMin, ref velocity, time);

            target.anchorMax
                = Vector3.SmoothDamp(target.anchorMax, nextMax, ref velocity, time);

            yield return null;
        }

        target.anchorMin = currentMin;
        target.anchorMax = currentMax;

        yield return null;
    }
}
