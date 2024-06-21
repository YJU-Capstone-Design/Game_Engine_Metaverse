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
    [SerializeField] List<GameObject> activeObjects; // 사용 후 상호작용이 불가능하게 만들 오브젝트들 (ex. 자물쇠)
    [SerializeField] List<GameObject> hintObjects; // 힌트 사용 시 상호작용하는 오브젝트들
    [SerializeField] List<GameObject> hintButtons; // 힌트 버튼들
    bool getSchoolsupplies = false;
    bool breakLock = false;
    [SerializeField] GameObject[] particles; // 힌트 사용 후 활성화 될 파티클

    [Header("# Player Info")]
    [SerializeField] TextMeshProUGUI timerText;
    public float playTime;
    public GameObject activeObjectButton;
    public TextMeshProUGUI activeObjectName;
    public bool interacting = false; // 현재 상호작용 중인지 여부
    public GameObject[] playerLife; // 플레이어 생명(하트) UI

    [Header("# Direction Lock")]
    public List<string> dirLockInput;
    string[] dirLockAnswer; // 방향 자물쇠 정답
    bool checkDirectioin = false;

    [Header("# Button Lock")]
    public List<int> btnLockInput;
    int[] btnLockAnswer; // 버튼 자물쇠 정답
    [SerializeField] RectTransform btnLockCheckButton; // 확인 버튼 -> 애니메이션 용도
    [SerializeField] ButtonLock[] btnLockButtons; // 클릭 가능한 버튼들

    [Header("# Dial Lock")]
    public List<int> dialLockInput;
    int[] dialLockAnswer;
    [SerializeField] DialLock[] dialLockTexts; // Dial Lock 숫자 칸

    [Header("# Key Lock")]
    [SerializeField] TextMeshProUGUI fluidKeyText; // "28 + 유저 수" 값을 가지는 키
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

        // 파티클 비활성화
        particles[0].SetActive(false);
        particles[1].SetActive(false);
        particles[2].SetActive(false);
    }

    private void Update()
    {
        // 방향 자물쇠 입력 값이 12개 일 경우 정답 확인
        if (dirLockInput.Count == 12 && !checkDirectioin)
        {
            Debug.Log(checkDirectioin);
            checkDirectioin = true;
            CheckLockAnswer("Direction");
        }

        // 켜져있는 오브젝트 꺼짐
        if (Input.GetKeyDown(KeyCode.Escape) && !isCheckAnswer)
        {
            // 화이트보드
            if (clueNote.activeInHierarchy)
            {
                clueNote.SetActive(false);

                foreach (GameObject clueNote in victimClueNotes)
                {
                    clueNote.SetActive(false);
                }
            }
            else if (narrationBox.activeInHierarchy && activeUIChildren[19].activeInHierarchy) // 냉장고 서랍칸
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

                    // 마지막 번호
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

        // 상호작용 가능한 오브젝트 버튼이 활성화 되었을 때
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

        // narrationBox 가 활성화 되었을 때, Enter 키를 누르면 기능이 있을 경우엔 기능 작동, 아니면 narrationBox 비활성화
        if (narrationBox.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (narrationText.text == narration.directionLock)
                {
                    activeUIChildren[7].SetActive(true); // 방향 자물쇠 UI 활성화
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.buttonLock)
                {
                    activeUIChildren[10].SetActive(true); // 버튼 자물쇠 UI 활성화
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.keyLock_2)
                {
                    activeUIChildren[12].SetActive(true); // 열쇠 자물쇠 UI 활성화
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.deadBodyBag)
                {
                    activeUIChildren[5].SetActive(true); // 지갑 UI 활성화
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
                    activeUIChildren[16].SetActive(true); // TV/Remote UI 활성화
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.doorLock)
                {
                    activeUIChildren[17].SetActive(true); // DoorLock UI 활성화
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.whiteBoard)
                {
                    activeUIChildren[18].SetActive(true); // WhiteBoard UI 활성화
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.refrigerator && !activeObjects[5].activeInHierarchy)
                {
                    activeUIChildren[19].SetActive(true); // 냉장고 UI 활성화
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
                    activeUIChildren[11].SetActive(true); // 번호 자물쇠 UI 활성화
                    narrationBox.SetActive(false);
                    pv.RPC("UsingLock", RpcTarget.All, "Button", true);

                    // SFX Sound
                    audioManager.SFX(0);
                    Debug.Log("Setting Button");
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                {
                    activeUIChildren[19].SetActive(true); // 냉장고 UI 활성화
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
        // Player List 정리
        photonManager.SetPlayerList();

        // fluidKeyText 설정
        fluidKeyText.text = (28 + photonManager.playerList.Count).ToString();
    }

    public void InGameSetting()
    {
        playTime = 900; // 15min

        // bool 값 초기화
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

    // 타이머 함수는 PhotonManager 에서 실행
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

            // 실패 UI 활성화
        }
    }

    void OpenNarration(string objName)
    {
        // narrationBox 활성화
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
        // 방향 자물쇠 초기화
        dirLockInput = new List<string>();

        // 방향 자물쇠 정답 세팅 상7, 우3, 하2 로 변경해야 됨.
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

    // 자물쇠 정답 확인
    public void CheckLockAnswer(string name)
    {
        Debug.Log("Check Lock");

        // 방향 자물쇠
        if (name == "Direction")
        {
            for (int i = 0; i < dirLockInput.Count; i++)
            {
                if (dirLockInput[i] != dirLockAnswer[i] || dirLockInput[i] == null)
                {
                    Debug.Log("실패");
                    dirLockInput.Clear(); // 입력 값 초기화

                    // 실패 로직
                    StartCoroutine(FailLock());
                    break;
                }
                else if (dirLockInput[dirLockInput.Count - 1] == dirLockAnswer[dirLockInput.Count - 1])
                {
                    Debug.Log("성공");

                    // 성공 로직
                    StartCoroutine(SuccessLock("Direction"));
                    break;
                }
            }
        }
        else if (name == "Button")
        {
            if (btnLockInput.Count > btnLockAnswer.Length || btnLockInput.Count < btnLockAnswer.Length)
            {
                Debug.Log("실패");
                btnLockInput.Clear(); // 입력 값 초기화

                // 실패 로직
                StartCoroutine(FailLock());
            }
            else if (btnLockInput.Count == btnLockAnswer.Length)
            {
                btnLockInput.Sort(); // 작은 순으로 정렬

                for (int i = 0; i < btnLockInput.Count; i++)
                {
                    if (btnLockInput[i] != btnLockAnswer[i])
                    {
                        Debug.Log("실패");
                        btnLockInput.Clear(); // 입력 값 초기화

                        // 실패 로직
                        StartCoroutine(FailLock());

                        break;
                    }
                    else if (btnLockInput[btnLockInput.Count - 1] == btnLockAnswer[btnLockInput.Count - 1])
                    {
                        Debug.Log("성공");

                        // 성공 로직
                        StartCoroutine(SuccessLock("Button"));
                    }
                }
            }

            // 버튼 애니메이션
            CheckButtonAnim();

            // 버튼 초기화
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
                    Debug.Log("실패");

                    // 실패 로직
                    StartCoroutine(FailLock());

                    break;
                }

                if (i == 2 && dialLockInput[dialLockInput.Count - 1] == dialLockAnswer[dialLockInput.Count - 1])
                {
                    Debug.Log("성공");

                    // 성공 로직
                    StartCoroutine(SuccessLock("Dial"));
                }
            }
        }
        else if (name == "TV")
        {
            if (tvInputText.text == tvAnswer)
            {
                Debug.Log("성공");

                // 성공 로직
                StartCoroutine(SuccessLock("TV"));
            }
            else
            {
                Debug.Log("실패");
                TVSetting();

                // 실패 로직
                StartCoroutine(FailLock());
            }
        }
        else if (name == "DoorLock")
        {
            if (doorLockInputText.text == doorLockAnswer)
            {
                Debug.Log("성공");

                StartCoroutine(SuccessLock("DoorLock"));
            }
            else
            {
                Debug.Log("실패");
                DoorLockSetting();

                // 실패 로직
                StartCoroutine(FailLock());
            }
        }
    }


    // 모든 자물쇠 값 초기화 함수
    void ResetInput(string lockName)
    {
        switch (lockName)
        {
            case "Direction":
                dirLockInput.Clear(); // 입력 값 초기화
                checkDirectioin = false;
                break;
            case "Button":
                btnLockInput.Clear();

                // 버튼 초기화
                foreach (ButtonLock btn in btnLockButtons)
                {
                    btn.upButton.SetActive(true);
                    btn.downButton.SetActive(false);
                }

                break;
            case "Dial":
                DialLockSetting(); // List 에 0,0,0 값이 그대로 있어야 하기에 Clear 를 사용하면 안됨.

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

    // 자물쇠 Reset 버튼
    public void ResetInputBtn(string name)
    {
        ResetInput(name);

        // SFX Sound
        audioManager.SFX(0);
        Debug.Log("Reset Lock Button");
    }

    // 버튼 자물쇠 확인 버튼 애니메이션
    public void CheckButtonAnim()
    {
        // Anchor 초기화
        Vector2 originalAnchorMin = new Vector2(0.4f, -0.05f);
        Vector2 originalAnchorMax = new Vector2(0.55f, 0);
        Vector2 nextAnchorMin = new Vector2(0.5f, -0.05f);
        Vector2 nextAnchorMax = new Vector2(0.65f, 0);

        StartCoroutine(SmoothCoroutine(btnLockCheckButton, originalAnchorMin, originalAnchorMax, nextAnchorMin, nextAnchorMax, 0.1f));
    }

    // 열쇠 자물쇠 Key 버튼
    public void UseKeyButton(bool answer)
    {
        if (answer)
        {
            Debug.Log("열쇠 자물쇠 성공");

            KeyLockSetting();

            // 성공 로직
            StartCoroutine(SuccessLock("Key"));
        }
        else
        {
            Debug.Log("열쇠 자물쇠 실패");

            // 실패 로직
            StartCoroutine(FailLock());
        }
    }

    // 문제 해결 시 실행되는 함수
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

        // UI 비활성화
        CloseAllUI();

        interacting = false;

        // 초기화
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

        // 성공 결과 (포톤)
        pv.RPC("OpenDoor", RpcTarget.All, name);
    }

    // 문제 해결 실패 시 실행되는 함수
    IEnumerator FailLock()
    {
        activeUIChildren[14].SetActive(true);

        isCheckAnswer = true;

        // SFX Sound
        audioManager.SFX(2);

        yield return new WaitForSeconds(1);

        // 방향 자물쇠
        checkDirectioin = false;

        isCheckAnswer = false;

        // 실패 UI 비활성화
        activeUIChildren[14].SetActive(false);

        // 실패 시 제한시간 30초 감소
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
                // TV 키패드 활성화
                getUSB = true;
                break;
            case "Dial":
                activeObjects[6].SetActive(false); // 현관 가벽 비활성화
                narration.refrigerator = "냉장고 아래 칸에 무언가 들어있는 거 같다.\n아래 칸을 살펴보길 원하시면 Enter를 눌러주십시오...";

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
                activeObjects[5].SetActive(false); // 주방 가벽 비활성화
                break;
            case "DoorLock":
                // 현관문 열리기
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

    // 화이트보드 ClueNote 버튼
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

    // 냉장고 서랍 칸 버튼
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

    // 힌트 사용
    [PunRPC]
    void UseHint()
    {
        if (photonManager.hintCount <= 0)
            return;

        // 남은 횟수가 1회일 시, 다른 사람 UI 도 종료
        //if (photonManager.hintCount == 1)
        //{
        //    narrationBox.SetActive(false);
        //    activeUIChildren[0].SetActive(false);
        //}

        // 힌트 사용 로직 필요
        if (photonManager.hintCount == 2)
        {
            // 옷장
            hintObjects[0].layer = 6;

            // 파티클 오브젝트 활성화 필요
            particles[0].SetActive(true);
            particles[1].SetActive(true);
        }
        else if (photonManager.hintCount == 1)
        {
            // 플레이어 가방
            hintObjects[1].layer = 6;

            // 파티클 오브젝트 활성화 필요
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

        particles[2].SetActive(false); // 파티클 비활성화

        // 플레이어 가방
        hintObjects[1].layer = 0;

    }



    // 인게임 힌트 버튼
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

    // Active UI 를 껐을 시, 초기화가 필요한 오브젝트들 초기화
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
            // 모든 자물쇠 초기화
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

            obj.transform.GetChild(1).GetComponent<Button>().enabled = true; // 신분증 버튼 기능 활성화 
        }
    }

    // 문제 버튼 클릭 이벤트
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

    // 지갑 애니메이션
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

            obj.GetComponent<Button>().enabled = false; // 버튼 기능은 비활성화 
        }

        // SFX Sound
        audioManager.SFX(0);
    }

    // 모든 UI 종료 버튼
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

            activeUIChildren[5].transform.GetChild(1).GetComponent<Button>().enabled = true; // 지갑 버튼 기능 활성화
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
