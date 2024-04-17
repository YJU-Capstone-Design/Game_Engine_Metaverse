using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] GameObject[] questionBox;

    [Header("# Wallet")]
    [SerializeField] GameObject[] walletObjs;

    private void Awake()
    {
        dirLockInput = new List<string>();

        dirLockAnswer = new string[4];
        dirLockAnswer[0] = "Up";
        dirLockAnswer[1] = "Down";
        dirLockAnswer[2] = "Right";
        dirLockAnswer[3] = "Left";
    }

    private void Update()
    {
        // ���� �ڹ��� �Է� ���� 4�� �� ��� ���� Ȯ��
        if(dirLockInput.Count == 4)
        {
            CheckAnswer(dirLockInput, dirLockAnswer);
        }
    }

    // ���� �ڹ��� ���� Ȯ��
    void CheckAnswer(List<string> input, string[] answer)
    {
        for(int i = 0; i < input.Count; i++)
        {
            if (input[i] != answer[i] || input[i] == null) 
            {
                Debug.Log("����");
                dirLockInput.Clear(); // �Է� �� �ʱ�ȭ
                break;
            }
            else if (input[input.Count - 1] == answer[input.Count - 1])
            {
                Debug.Log("����");
                dirLockInput.Clear();
            }
        }
    }

    // ���� �ڹ��� �� �ʱ�ȭ
    public void ResetInput()
    {
        dirLockInput.Clear(); // �Է� �� �ʱ�ȭ
    }

    // ���� UI �� ���� ��, ��� ��ư ���� �ʱ�ȭ
    void CloseQueBox()
    {
        foreach(GameObject btn in answerBtns)
        {
            Image btnImage = btn.GetComponent<Image>();
            TextMeshProUGUI text = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            btnImage.sprite = btnSprites[0];
            text.color = Color.white;
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

    public void CheckIDCard(GameObject obj)
    {
        Animator anim = obj.GetComponent<Animator>();

        if (anim != null)
        {
            foreach(GameObject wallet in walletObjs) {
                anim = wallet.GetComponent<Animator>();
                anim.SetBool("Click", true);
            }
        }
    }
}
