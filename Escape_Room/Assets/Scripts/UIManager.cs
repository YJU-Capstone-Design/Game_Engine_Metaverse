using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public List<string> dirLockInput;
    string[] dirLockAnswer; // ���� �ڹ��� ����

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

    public void ResetInput()
    {
        dirLockInput.Clear(); // �Է� �� �ʱ�ȭ
    }
}
