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

    [Header("# Button Lock")]
    public List<int> btnLockInput;
    int[] btnLockAnswer; // ��ư �ڹ��� ����
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
        // ���� �ڹ��� �Է� ���� 4�� �� ��� ���� Ȯ��
        if (dirLockInput.Count == 4)
        {
            CheckLockAnswer("Direction");
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

        // �ڹ���� ��ȣ�ۿ��� ���� ��, Enter Ű�� ������ �ش� �ڹ��� UI Ȱ��ȭ
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
        // ���� �ڹ��� �ʱ�ȭ
        dirLockInput = new List<string>();

        // ���� �ڹ��� ���� ���� ��7, ��3, ��2 �� �����ؾ� ��.
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
                    break;
                }
                else if (dirLockInput[dirLockInput.Count - 1] == dirLockAnswer[dirLockInput.Count - 1])
                {
                    Debug.Log("����");
                    dirLockInput.Clear();

                    // UI ����
                    foreach (GameObject obj in activeUIChildren)
                    {
                        if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
                    }

                    // ���� �� ��ȣ�ۿ� ���� �ʿ�
                }
            }
        }
        else if (name == "Button")
        {
            if (btnLockInput.Count > btnLockAnswer.Length || btnLockInput.Count < btnLockAnswer.Length)
            {
                Debug.Log("����");
                btnLockInput.Clear(); // �Է� �� �ʱ�ȭ
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
                        break;
                    }
                    else if (btnLockInput[btnLockInput.Count - 1] == btnLockAnswer[btnLockInput.Count - 1])
                    {
                        Debug.Log("����");
                        btnLockInput.Clear();

                        // UI ����
                        foreach (GameObject obj in activeUIChildren)
                        {
                            if (obj.activeInHierarchy) { CloseAcvtiveUI(obj); obj.SetActive(false); }
                        }

                        // ���� �� ��ȣ�ۿ� ���� �ʿ�
                    }
                }
            }

            // ��ư �ִϸ��̼�
            CheckButtonAnim();

            // ��ư �ʱ�ȭ
            foreach(ButtonLock btn in btnLockButtons)
            {
                btn.upButton.SetActive(true);
                btn.downButton.SetActive(false);
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
                break;
        }
    }

    // ä�� ���� ���� ��ư
    public void CheckButtonAnim()
    {
        // Anchor �ʱ�ȭ
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
