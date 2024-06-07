using Photon.Pun;
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

    [Header("# Player Info")]
    [SerializeField] TextMeshProUGUI timerText;
    public float playTime; 
    public GameObject activeObjectButton;
    public TextMeshProUGUI activeObjectName;
    public bool interacting = false; // 현재 상호작용 중인지 여부

    [Header("# Direction Lock")]
    public List<string> dirLockInput;
    string[] dirLockAnswer; // 방향 자물쇠 정답

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

    [Header("# Question Button")]
    [SerializeField] GameObject[] answerBtns;
    [SerializeField] Sprite[] btnSprites;

    [Header("# Wallet")]
    [SerializeField] GameObject[] walletObjs;

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
    }

    private void Update()
    {
        // 방향 자물쇠 입력 값이 4개 일 경우 정답 확인
        if (dirLockInput.Count == 4)
        {
            CheckLockAnswer("Direction");
        }

        // 켜져있는 오브젝트 꺼짐
        if (Input.GetKeyDown(KeyCode.Escape) && !isCheckAnswer)
        {
            narrationText.text = "";

            CloseAllUI();
        }

        // 상호작용 가능한 오브젝트 버튼이 활성화 되었을 때
        if(activeObjectButton.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                OpenNarration(activeObjectName.text); 
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
                else if (narrationText.text == narration.dialLock)
                {
                    activeUIChildren[11].SetActive(true); // 번호 자물쇠 UI 활성화
                    narrationBox.SetActive(false);
                }
                else if (narrationText.text == narration.keyLock)
                {
                    activeUIChildren[12].SetActive(true); // 열쇠 자물쇠 UI 활성화
                    narrationBox.SetActive(false);
                }
                else if(narrationText.text == narration.deadBodyBag)
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
                else
                {
                    interacting = false;

                    CloseAllUI();
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
        playTime = 1800;

        foreach(GameObject activeObj in activeObjects)
        {
            activeObj.GetComponent<Collider>().enabled = true;
        }
    }

    // 타이머 함수는 PhotonManager 에서 실행
    [PunRPC]
    void Timer(float time)
    {
        if(time > 0)
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
                break;
            case "ButtonLock":
                narrationText.text = narration.buttonLock;
                break;
            case "Refrigerator":
                narrationText.text = narration.dialLock;
                break;
            case "KeyLock":
                narrationText.text = narration.keyLock;
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
        }
    }

    void DirectionLockSetting()
    {
        // 방향 자물쇠 초기화
        dirLockInput = new List<string>();

        // 방향 자물쇠 정답 세팅 상7, 우3, 하2 로 변경해야 됨.
        dirLockAnswer = new string[4] { "Up", "Down", "Right", "Left" };
    }

    void ButtonLockSetting()
    {
        btnLockInput = new List<int>();

        btnLockAnswer = new int[4] { 1, 2, 4, 5 };
    }

    void DialLockSetting()
    {
        dialLockInput = new List<int> { 0, 0, 0 };

        dialLockAnswer = new int[3] { 7, 3, 2 };
    }

    // 자물쇠 정답 확인
    public void CheckLockAnswer(string name)
    {
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
                    dirLockInput.Clear();

                    // 성공 로직
                    StartCoroutine(SuccessLock("Direction"));
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
                        btnLockInput.Clear();

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
                    DialLockSetting(); // 초기화

                    // 성공 로직
                    StartCoroutine(SuccessLock("Dial"));
                }
            }
        }
    }

    // 방자물쇠 값 초기화
    public void ResetInput(string lockName)
    {
        switch (lockName)
        {
            case "Direction":
                dirLockInput.Clear(); // 입력 값 초기화
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
        }
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
        if(answer)
        {
            Debug.Log("열쇠 자물쇠 성공");

            activeUIChildren[12].SetActive(false); // 열쇠 자물쇠 UI 비활성화
            activeUIChildren[0].SetActive(false);

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

        yield return new WaitForSeconds(1);

        isCheckAnswer = false;

        // UI 비활성화
        CloseAllUI();

        // 성공 결과 (포톤)
        pv.RPC("OpenDoor", RpcTarget.All, name);

        interacting = false;

        // 사운드도 필요
    }

    // 문제 해결 실패 시 실행되는 함수
    IEnumerator FailLock()
    {
        activeUIChildren[14].SetActive(true);

        isCheckAnswer = true;

        yield return new WaitForSeconds(1);

        isCheckAnswer = false;

        // 실패 UI 비활성화
        activeUIChildren[14].SetActive(false);

        // 실패 시 제한시간 30초 감소
        pv.RPC("ReduceTime", RpcTarget.MasterClient);
    }

    public void BreakLockButton(string name)
    {
        StartCoroutine(SuccessLock(name));

        pv.RPC("BreakLock", RpcTarget.All);
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
                doorAnim = doors[0].GetComponent<Animator>();
                doorAnim.SetBool("open", true);
                activeObjects[0].GetComponent<Collider>().enabled = false;
                break;
            case "Button":
                break;
            case "Dial":
                break;
            case "Key":
                break;
        }
    }

    [PunRPC]
    void ReduceTime()
    {
        playTime -= 30;
    }

    // 힌트 사용
    [PunRPC]
    void UseHint()
    {
        if (photonManager.hintCount <= 0)
            return;

        // 남은 횟수가 1회일 시, 다른 사람 UI 도 종료
        if (photonManager.hintCount == 1)
        {
            narrationBox.SetActive(false);
            activeUIChildren[0].SetActive(false);
        }

        // 힌트 사용 로직 필요
        if(photonManager.hintCount == 2)
        {
            hintObjects[0].layer = 6;

            // 파티클 오브젝트 활성화 필요
        }
        else if(photonManager.hintCount == 1)
        {
            hintObjects[1].layer = 6;

            // 파티클 오브젝트 활성화 필요
        }

        photonManager.hintCount--;
    }

    [PunRPC]
    void GetSchoolsupplies()
    {
        getSchoolsupplies = true;

        foreach(GameObject hintButton in hintButtons)
        {
            hintButton.SetActive(true); ;
        }
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
            ResetInput("Direction");
            ResetInput("Button");
            // 다른 자물쇠 Input 도 필요
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
    }

    // 게임 시작 후 Lobby 로 돌아가는 버튼 재확인 UI
    public void OpenBackToLobbyUI()
    {
        activeUIChildren[15].SetActive(true);

        interacting = true;
    }

    // 모든 UI 종료 버튼
    public void CloseAllUI()
    {
        foreach (GameObject obj in activeUIChildren)
        {
            if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }

            activeUIChildren[5].transform.GetChild(1).GetComponent<Button>().enabled = true; // 지갑 버튼 기능 활성화
        }

        interacting = false;

        narrationText.text = "";
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
