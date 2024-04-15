using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public List<string> dirLockInput;
    string[] dirLockAnswer; // 방향 자물쇠 정답

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

    // 방향 자물쇠 정답 확인
    void CheckAnswer(List<string> input, string[] answer)
    {
        for(int i = 0; i < input.Count; i++)
        {
            if (input[i] != answer[i] || input[i] == null) 
            {
                Debug.Log("실패");
                dirLockInput.Clear(); // 입력 값 초기화
                break;
            }
            else if (input[input.Count - 1] == answer[input.Count - 1])
            {
                Debug.Log("성공");
                dirLockInput.Clear();
            }
        }
    }

    public void ResetInput()
    {
        dirLockInput.Clear(); // 입력 값 초기화
    }
}
