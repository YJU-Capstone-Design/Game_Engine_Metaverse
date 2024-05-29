using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLock : MonoBehaviour
{
    public GameObject downButton;
    public GameObject upButton;

    UIManager uiManager;

    private void Awake()
    {
        uiManager = UIManager.Instance;

        upButton.SetActive(true);
        downButton.SetActive(false);
    }

    private void OnDisable()
    {
        upButton.SetActive(true);
        downButton.SetActive(false);
    }

    public void ClickButton(int num)
    {
        // 활성화된 버튼을 넣었을 때
        if(downButton.activeInHierarchy)
        {
            upButton.SetActive(true);
            downButton.SetActive(false);

            if (uiManager.btnLockInput.Contains(num)) { uiManager.btnLockInput.Remove(num); }
        }
        else // 버튼을 눌렀을 때
        {
            upButton.SetActive(false);
            downButton.SetActive(true);

            if(!uiManager.btnLockInput.Contains(num)) { uiManager.btnLockInput.Add(num); }
        }
    }
}
