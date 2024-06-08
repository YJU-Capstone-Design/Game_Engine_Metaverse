using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoteButton : MonoBehaviour
{
    Image img;
    UIManager uiManager;

    private void Awake()
    {
        img = GetComponent<Image>();
        img.enabled = false;

    }
    private void Start()
    {
        uiManager = UIManager.Instance;
    }

    public void InputButton(string value)
    {
        if(value == "Delete")
        {
            if(uiManager.tvInputText.text.Length > 0)
            {
                int index = uiManager.tvInput.Count;
                uiManager.tvInput.Remove(uiManager.tvInput[index - 1]);
                uiManager.tvInputText.text = string.Join("", uiManager.tvInput);

            }
        }
        else if(value == "null")
        {
            Debug.Log("null");
        }
        else
        {
            uiManager.tvInput.Add(value);
            uiManager.tvInputText.text = string.Join("", uiManager.tvInput);
        }

        StartCoroutine(EnableImage());
    }

    IEnumerator EnableImage()
    {
        img.enabled = true;

        yield return new WaitForSeconds(0.2f);

        img.enabled = false;
    }
}
