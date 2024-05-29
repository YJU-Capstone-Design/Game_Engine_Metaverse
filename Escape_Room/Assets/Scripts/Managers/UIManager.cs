using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("# Direction Lock")]
    public List<string> dirLockInput;
    string[] dirLockAnswer; // 방향 자물쇠 정답

    [Header("# Button Lock")]
    public List<int> btnLockInput;
    int[] btnLockAnswer; // 버튼 자물쇠 정답
    [SerializeField] RectTransform btnLockCheckButton;
    [SerializeField] ButtonLock[] btnLockButtons;

    [Header("# Button")]
    [SerializeField] GameObject[] answerBtns;
    [SerializeField] Sprite[] btnSprites;

    [Header("# UI Boxs")]
    [SerializeField] List<GameObject> activeUIChildren;

    [Header("# Wallet")]
    [SerializeField] GameObject[] walletObjs;

    [Header("# Narration")]
    Narration narration;
    [SerializeField] GameObject narrationBox;
    [SerializeField] TextMeshProUGUI narrationText;

    private void Awake()
    {
        narration = GetComponent<Narration>();

        DirectionLockSetting();
        ButtonLockSetting();
    }

    private void Update()
    {
        // 방향 자물쇠 입력 값이 4개 일 경우 정답 확인
        if (dirLockInput.Count == 4)
        {
            CheckLockAnswer("Direction");
        }

        // 켜져있는 오브젝트 꺼짐
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject obj in activeUIChildren)
            {
                if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
            }
        }

        // 테스트 용 나레이션 오픈 로직
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            narrationBox.SetActive(true);
            activeUIChildren[0].SetActive(true);
            narrationText.text = narration.keyLock;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            narrationBox.SetActive(true);
            activeUIChildren[0].SetActive(true);
            narrationText.text = narration.directionLock;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            narrationBox.SetActive(true);
            activeUIChildren[0].SetActive(true);
            narrationText.text = narration.dialLock;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            narrationBox.SetActive(true);
            activeUIChildren[0].SetActive(true);
            narrationText.text = narration.buttonLock;
        }

        // 자물쇠와 상호작용을 했을 때, Enter 키를 누르면 해당 자물쇠 UI 활성화
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (narrationText.text == narration.directionLock)
            {
                narrationBox.SetActive(false);
                activeUIChildren[7].SetActive(true);
            }
            else if (narrationText.text == narration.buttonLock)
            {
                narrationBox.SetActive(false);
                activeUIChildren[10].SetActive(true);
            }
        }
    }

    void DirectionLockSetting()
    {
        // 방향 자물쇠 초기화
        dirLockInput = new List<string>();

        // 방향 자물쇠 정답 세팅 상7, 우3, 하2 로 변경해야 됨.
        dirLockAnswer = new string[4];
        dirLockAnswer[0] = "Up";
        dirLockAnswer[1] = "Down";
        dirLockAnswer[2] = "Right";
        dirLockAnswer[3] = "Left";
    }

    void ButtonLockSetting()
    {
        btnLockInput = new List<int>();

        btnLockAnswer = new int[4];
        btnLockAnswer[0] = 1;
        btnLockAnswer[1] = 2;
        btnLockAnswer[2] = 4;
        btnLockAnswer[3] = 5;
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
                    break;
                }
                else if (dirLockInput[dirLockInput.Count - 1] == dirLockAnswer[dirLockInput.Count - 1])
                {
                    Debug.Log("성공");
                    dirLockInput.Clear();

                    // UI 종료
                    foreach (GameObject obj in activeUIChildren)
                    {
                        if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
                    }

                    // 성공 시 상호작용 로직 필요
                }
            }
        }
        else if (name == "Button")
        {
            if (btnLockInput.Count > btnLockAnswer.Length || btnLockInput.Count < btnLockAnswer.Length)
            {
                Debug.Log("실패");
                btnLockInput.Clear(); // 입력 값 초기화
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
                        break;
                    }
                    else if (btnLockInput[btnLockInput.Count - 1] == btnLockAnswer[btnLockInput.Count - 1])
                    {
                        Debug.Log("성공");
                        btnLockInput.Clear();

                        // UI 종료
                        foreach (GameObject obj in activeUIChildren)
                        {
                            if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
                        }

                        // 성공 시 상호작용 로직 필요
                    }
                }
            }

            // 버튼 애니메이션
            CheckButtonAnim();

            // 버튼 초기화
            foreach(ButtonLock btn in btnLockButtons)
            {
                btn.upButton.SetActive(true);
                btn.downButton.SetActive(false);
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
                break;
        }
    }

    // 채팅 정보 열람 버튼
    public void CheckButtonAnim()
    {
        // Anchor 초기화
        Vector2 originalAnchorMin = new Vector2(0.4f, -0.05f);
        Vector2 originalAnchorMax = new Vector2(0.55f, 0);
        Vector2 nextAnchorMin = new Vector2(0.5f, -0.05f);
        Vector2 nextAnchorMax = new Vector2(0.65f, 0);

        StartCoroutine(SmoothCoroutine(btnLockCheckButton, originalAnchorMin, originalAnchorMax, nextAnchorMin, nextAnchorMax, 0.1f));
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
        obj.GetComponent<Button>().enabled = false; // 버튼 기능은 비활성화    

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
