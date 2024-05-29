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
    string[] dirLockAnswer; // ���� �ڹ��� ����

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
        // ���� �ڹ��� �Է� ���� 4�� �� ��� ���� Ȯ��
        if (dirLockInput.Count == 4)
        {
            CheckAnswer(dirLockInput, dirLockAnswer);
        }

        // �����ִ� ������Ʈ ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject obj in activeUIChildren)
            {
                if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
            }
        }

        // �׽�Ʈ �� �����̼� ���� ����
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

        // �ڹ���� ��ȣ�ۿ��� ���� ��, Enter Ű�� ������ �ش� �ڹ��� UI Ȱ��ȭ
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
        // ���� �ڹ��� �ʱ�ȭ
        dirLockInput = new List<string>();

        // ���� �ڹ��� ���� ����
        dirLockAnswer = new string[4];
        dirLockAnswer[0] = "Up";
        dirLockAnswer[1] = "Down";
        dirLockAnswer[2] = "Right";
        dirLockAnswer[3] = "Left";
    }

    // ���� �ڹ��� ���� Ȯ��
    void CheckAnswer(List<string> input, string[] answer)
    {
        for(int i = 0; i < input.Count; i++)
        {
            if (input[i] != answer[i] || input[i] == null) 
            {
                Debug.Log("����");
                input.Clear(); // �Է� �� �ʱ�ȭ
                break;
            }
            else if (input[input.Count - 1] == answer[input.Count - 1])
            {
                Debug.Log("����");
                input.Clear();

                // UI ����
                foreach (GameObject obj in activeUIChildren)
                {
                    if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
                }

                // ���� �� ��ȣ�ۿ� ���� �ʿ�
            }
        }
    }

    // ���� �ڹ��� �� �ʱ�ȭ
    public void ResetInput(string lockName)
    {
        switch (lockName)
        {
            case "Direction":
                dirLockInput.Clear(); // �Է� �� �ʱ�ȭ
                break;
        }
    }

    // Active UI �� ���� ��, �ʱ�ȭ�� �ʿ��� ������Ʈ�� �ʱ�ȭ
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
            // �ٸ� �ڹ��� Input �� �ʿ�
        } 
        else if(obj.name.Contains("Wallet"))
        {
            for(int i = 0; i < 3; i++)
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

    // ���� �ִϸ��̼�
    public void CheckIDCard(GameObject obj)
    {
        Animator anim = obj.GetComponent<Animator>();
        obj.GetComponent<Button>().enabled = false; // ��ư ����� ��Ȱ��ȭ    

        if (anim != null)
        {
            foreach(GameObject wallet in walletObjs) {
                anim = wallet.GetComponent<Animator>();
                anim.SetBool("Click", true);
            }
        }
    }
}
