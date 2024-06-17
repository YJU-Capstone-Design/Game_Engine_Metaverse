using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorLockButton : MonoBehaviour
{
    [SerializeField] Sprite whiteButton;
    [SerializeField] Sprite grayButton;

    Image img;
    UIManager uiManager;
    Animator anim;

    private void Awake()
    {
        img = GetComponent<Image>();

        if (gameObject.name.Contains("Handle"))
        {
            anim = GetComponent<Animator>();
        }
    }
    private void Start()
    {
        uiManager = UIManager.Instance;
    }

    public void InputButton(string value)
    {
        if (value == "Delete")
        {
            if (uiManager.doorLockInputText.text.Length > 0)
            {
                int index = uiManager.doorLockInput.Count;
                uiManager.doorLockInput.Remove(uiManager.doorLockInput[index - 1]);
                uiManager.doorLockInputText.text = string.Join("", uiManager.doorLockInput);

            }
        }
        else if (value == "null")
        {
            Debug.Log("null");
        }
        else if(value == "Handle")
        {
            Debug.Log("anim");
            anim.SetBool("check", true);
            StartCoroutine(HandleAnimation());

            // SFX Sound
            AudioManager.Instance.SFX(5);
        }
        else
        {
            uiManager.doorLockInput.Add(value);
            uiManager.doorLockInputText.text = string.Join("", uiManager.doorLockInput);
        }

        if(!gameObject.name.Contains("Handle"))
        {
            StartCoroutine(EnableImage());

            // SFX Sound
            AudioManager.Instance.SFX(0);
            Debug.Log("DoorLock Handle");
        }
    }

    IEnumerator EnableImage()
    {
        img.sprite = grayButton;

        yield return new WaitForSeconds(0.2f);

        img.sprite = whiteButton;
    }

    IEnumerator HandleAnimation()
    {
        yield return new WaitForSeconds(0.8f);

        if (gameObject.name.Contains("Handle"))
        {
            anim.SetBool("check", false);
        }
    }
}
