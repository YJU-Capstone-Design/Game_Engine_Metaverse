using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialLock : MonoBehaviour
{
    public TextMeshProUGUI leftNumText;
    public TextMeshProUGUI middleNumText;
    public TextMeshProUGUI rightNumText;

    int leftNum;
    int middleNum;
    int rightNum;

    private void Awake()
    {
        SetInitialValue();
    }

    private void OnDisable()
    {
        SetInitialValue();

        leftNumText.text = leftNum.ToString();
        middleNumText.text = middleNum.ToString();
        rightNumText.text = rightNum.ToString();

        UIManager.Instance.dialLockInput = new List<int> { 0, 0, 0 };
    }

    public void SetInitialValue()
    {
        leftNumText.text = "9";
        middleNumText.text = "0";
        rightNumText.text = "1";

        leftNum = 9;
        middleNum = 0;
        rightNum = 1;
    }

    public void LeftButton(int index)
    {
        if(leftNum == 0) { leftNum = 9; } else { leftNum--; }
        if (middleNum == 0) { middleNum = 9; } else { middleNum--; }
        if (rightNum == 0) { rightNum = 9; } else { rightNum--; }

        leftNumText.text = leftNum.ToString();
        middleNumText.text = middleNum.ToString();
        rightNumText.text = rightNum.ToString();

        UIManager.Instance.dialLockInput[index] = middleNum;

        // SFX Sound
        AudioManager.Instance.SFX(0);
        Debug.Log("Dial Left Button");
    }

    public void RightButton(int index)
    {
        if (leftNum == 9) { leftNum = 0; } else { leftNum++; }
        if (middleNum == 9) { middleNum = 0; } else { middleNum++; }
        if (rightNum == 9) { rightNum = 0; } else { rightNum++; }

        leftNumText.text = leftNum.ToString();
        middleNumText.text = middleNum.ToString();
        rightNumText.text = rightNum.ToString();

        UIManager.Instance.dialLockInput[index] = middleNum;

        // SFX Sound
        AudioManager.Instance.SFX(0);
        Debug.Log("Dial Right Button");
    }
}
