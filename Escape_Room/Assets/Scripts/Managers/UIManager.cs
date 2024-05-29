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

    [Header("# Button")]
    [SerializeField] GameObject[] answerBtns;
    [SerializeField] Sprite[] btnSprites;

    [Header("# UI Boxs")]
    [SerializeField] List<GameObject> activeUIChildren;

    [Header("# Wallet")]
    [SerializeField] GameObject[] walletObjs;

    [Header("Narration")]
    Narration narration;
    [SerializeField] GameObject narrationBox;
    [SerializeField] TextMeshProUGUI narrationText;

    private void Awake()
    {
        narration = GetComponent<Narration>();

        DirectionLock();
    }

    private void Update()
    {
        // 방향 자물쇠 입력 값이 4개 일 경우 정답 확인
        if (dirLockInput.Count == 4)
        {
            CheckAnswer(dirLockInput, dirLockAnswer);
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
        if(Input.GetKeyDown(KeyCode.Keypad1))
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
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (narrationText.text == narration.directionLock)
            {
                narrationBox.SetActive(false);
                activeUIChildren[7].SetActive(true);
            }
        }
    }

    void DirectionLock()
    {
        // 방향 자물쇠 초기화
        dirLockInput = new List<string>();

        // 방향 자물쇠 정답 세팅
        dirLockAnswer = new string[4];
        dirLockAnswer[0] = "Up";
        dirLockAnswer[1] = "Down";
        dirLockAnswer[2] = "Right";
        dirLockAnswer[3] = "Left";
    }

    // 방향 자물쇠 정답 확인
    void CheckAnswer(List<string> input, string[] answer)
    {
        for(int i = 0; i < input.Count; i++)
        {
            if (input[i] != answer[i] || input[i] == null) 
            {
                Debug.Log("실패");
                input.Clear(); // 입력 값 초기화
                break;
            }
            else if (input[input.Count - 1] == answer[input.Count - 1])
            {
                Debug.Log("성공");
                input.Clear();

                // UI 종료
                foreach (GameObject obj in activeUIChildren)
                {
                    if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
                }

                // 성공 시 상호작용 로직 필요
            }
        }
    }

    // 방향 자물쇠 값 초기화
    public void ResetInput(string lockName)
    {
        switch (lockName)
        {
            case "Direction":
                dirLockInput.Clear(); // 입력 값 초기화
                break;
        }
    }

    // Active UI 를 껐을 시, 초기화가 필요한 오브젝트들 초기화
    void CloseAcvtiveUI(GameObject obj)
    {
        if(obj.name.Contains("Question"))
        {
            foreach (GameObject btn in answerBtns)
            {
                Image btnImage = btn.GetComponent<Image>();
                TextMeshProUGUI text = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                btnImage.sprite = btnSprites[0];
                text.color = Color.white;
            }
        } 
        else if(obj.name.Contains("Lock"))
        {
            ResetInput("Direction");
            // 다른 자물쇠 Input 도 필요
        } 
        else if(obj.name.Contains("Wallet"))
        {
            for(int i = 0; i < 3; i++)
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
        for(int i = 0; i < answerBtns.Length; i++)
        {
            Image btnImage;
            TextMeshProUGUI text;

            if (i == index)
            {
                btnImage = answerBtns[index].GetComponent<Image>();
                text = answerBtns[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                btnImage.sprite = btnSprites[1];
                text.color = Color.black;
            } else
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
            foreach(GameObject wallet in walletObjs) {
                anim = wallet.GetComponent<Animator>();
                anim.SetBool("Click", true);
            }
        }
    }
}
