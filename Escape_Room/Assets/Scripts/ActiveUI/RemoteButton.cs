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
        Debug.Log(gameObject.name);
        if (!uiManager.connetUSB)
            return;

        if(value == "Delete")
        {
            if(uiManager.tvInputText.text.Length > 0)
            {
                int index = uiManager.tvInput.Count;
                uiManager.tvInput.Remove(uiManager.tvInput[index - 1]);
                uiManager.tvInputText.text = string.Join("", uiManager.tvInput);

            }
        }
        else if(value == "Power")
        {
            uiManager.tvInputField.SetActive(true);
            uiManager.tvPowerOn = true;
        }
        else if(value == "null")
        {
            Debug.Log("null");
        }
        else
        {
            if (!uiManager.tvPowerOn)
                return;
            uiManager.tvInput.Add(value);
            uiManager.tvInputText.text = string.Join("", uiManager.tvInput);
        }

        // SFX Sound
        AudioManager.Instance.SFX(0);

        StartCoroutine(EnableImage());
    }

    IEnumerator EnableImage()
    {
        img.enabled = true;

        yield return new WaitForSeconds(0.2f);

        img.enabled = false;
    }
}
