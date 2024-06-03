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

    [Header("# Player Info")]
    [SerializeField] TextMeshProUGUI timerText;
    public float playTime; 
    public GameObject activeObjectButton;
    public TextMeshProUGUI activeObjectName;
    public bool interacting = false; // ���� ��ȣ�ۿ� ������ ����

    [Header("# Direction Lock")]
    public List<string> dirLockInput;
    string[] dirLockAnswer; // ���� �ڹ��� ����

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
        // ���� �ڹ��� �Է� ���� 4�� �� ��� ���� Ȯ��
        if (dirLockInput.Count == 4)
        {
            CheckLockAnswer("Direction");
        }

        // �����ִ� ������Ʈ ����
        if (Input.GetKeyDown(KeyCode.Escape) && !isCheckAnswer)
        {
            foreach (GameObject obj in activeUIChildren)
            {
                if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
            }

            interacting = false;
        }

        // ��ȣ�ۿ� ������ ������Ʈ ��ư�� Ȱ��ȭ �Ǿ��� ��
        if(activeObjectButton.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                // narrationBox Ȱ��ȭ
                narrationBox.SetActive(true);
                activeUIChildren[0].SetActive(true);

                interacting = true;

                switch (activeObjectName.text)
                {
                    case "ħ��":
                        narrationText.text = narration.bed;
                        break;
                    case "��ü":
                        narrationText.text = narration.deadBody;
                        break;
                    case "����":
                        narrationText.text = narration.chair;
                        break;
                    case "��Ʈ��":
                        narrationText.text = narration.laptop;
                        // ��Ʈ�� UI �� ���� Ȱ��ȭ??
                        break;
                    case "����":
                        narrationText.text = narration.document;
                        break;
                    case "�� ����":
                        narrationText.text = narration.playerBag;
                        break;
                    case "������ ����":
                        narrationText.text = narration.deadBodyBag;
                        break;
                    case "����":
                        narrationText.text = narration.wallet;
                        break;
                    case "������ �ð�":
                        narrationText.text = narration.wallClock;
                        break;
                    case "��Į":
                        narrationText.text = narration.kitchenKnife;
                        break;
                    case "������ TV":
                        narrationText.text = narration.WallTV;
                        break;
                    case "���� �ڹ���":
                        narrationText.text = narration.directionLock;
                        break;
                    case "��ư �ڹ���":
                        narrationText.text = narration.buttonLock;
                        break;
                    case "���̾� �ڹ���":
                        narrationText.text = narration.dialLock;
                        break;
                    case "���� �ڹ���":
                        narrationText.text = narration.keyLock;
                        break;
                    case "������":
                        narrationText.text = narration.storageCloset;
                        break;
                }
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
                }
                else if (narrationText.text == narration.buttonLock)
                {
                    activeUIChildren[10].SetActive(true); // ��ư �ڹ��� UI Ȱ��ȭ
                }
                else if (narrationText.text == narration.dialLock)
                {
                    activeUIChildren[11].SetActive(true); // ��ȣ �ڹ��� UI Ȱ��ȭ
                }
                else if (narrationText.text == narration.keyLock)
                {
                    activeUIChildren[12].SetActive(true); // ���� �ڹ��� UI Ȱ��ȭ
                }
                else if (narrationText.text == narration.hint)
                {
                    if (photonManager.hintCount > 0)
                    {
                        pv.RPC("UseHint", RpcTarget.All);
                    }

                    interacting = false;
                }
                else
                {
                    interacting = false;
                }

                narrationBox.SetActive(false);
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

    void InGameSetting()
    {
        playTime = 1800;
    }

    // Ÿ�̸� �Լ��� PhotonManager ���� ����
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

            // ���� UI Ȱ��ȭ
        }
    }

    void DirectionLockSetting()
    {
        // ���� �ڹ��� �ʱ�ȭ
        dirLockInput = new List<string>();

        // ���� �ڹ��� ���� ���� ��7, ��3, ��2 �� �����ؾ� ��.
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

    // �ڹ��� ���� Ȯ��
    public void CheckLockAnswer(string name)
    {
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
                    dirLockInput.Clear();

                    // ���� ����
                    StartCoroutine(SuccessLock("Direction"));
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
                        btnLockInput.Clear();

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
                    DialLockSetting(); // �ʱ�ȭ

                    // ���� ����
                    StartCoroutine(SuccessLock("Dial"));
                }
            }
        }
    }

    // ���ڹ��� �� �ʱ�ȭ
    public void ResetInput(string lockName)
    {
        switch (lockName)
        {
            case "Direction":
                dirLockInput.Clear(); // �Է� �� �ʱ�ȭ
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
        }
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
        if(answer)
        {
            Debug.Log("���� �ڹ��� ����");

            activeUIChildren[12].SetActive(false); // ���� �ڹ��� UI ��Ȱ��ȭ
            activeUIChildren[0].SetActive(false);

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

        yield return new WaitForSeconds(1);

        isCheckAnswer = false;

        // UI ��Ȱ��ȭ
        foreach (GameObject obj in activeUIChildren)
        {
            if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
        }

        interacting = false;

        // ���� ��� ���� �ʿ�
        switch (name)
        {
            case "Direction":
                break;
            case "Button":
                break;
            case "Dial":
                break;
            case "Key":
                break;
        }

        // ���嵵 �ʿ�
    }

    // ���� �ذ� ���� �� ����Ǵ� �Լ�
    IEnumerator FailLock()
    {
        activeUIChildren[14].SetActive(true);

        isCheckAnswer = true;

        yield return new WaitForSeconds(1);

        isCheckAnswer = false;

        // ���� UI ��Ȱ��ȭ
        activeUIChildren[14].SetActive(false);

        // ���� �� ���ѽð� 30�� ����
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

    // ��Ʈ ���
    [PunRPC]
    void UseHint()
    {
        if (photonManager.hintCount <= 0)
            return;

        // ���� Ƚ���� 1ȸ�� ��, �ٸ� ��� UI �� ����
        if (photonManager.hintCount == 1)
        {
            narrationBox.SetActive(false);
            activeUIChildren[0].SetActive(false);
        }

        photonManager.hintCount--;

        Debug.Log("Use Hint");
        // ��Ʈ ��� ���� �ʿ�
    }

    public void HintButton()
    {
        if (photonManager.hintCount > 0)
        {
            narrationText.text = narration.hint;
            narrationBox.SetActive(true);
            activeUIChildren[0].SetActive(true);
        }
        else
        {
            narrationText.text = narration.hintZero;
            narrationBox.SetActive(true);
            activeUIChildren[0].SetActive(true);
        }
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
            ResetInput("Direction");
            ResetInput("Button");
            // �ٸ� �ڹ��� Input �� �ʿ�
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
        obj.GetComponent<Button>().enabled = false; // ��ư ����� ��Ȱ��ȭ    

        if (anim != null)
        {
            foreach (GameObject wallet in walletObjs)
            {
                anim = wallet.GetComponent<Animator>();
                anim.SetBool("Click", true);
            }
        }
    }
}
