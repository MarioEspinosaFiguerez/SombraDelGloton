using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ScrollButton : MonoBehaviour
{
    public Animator scrollAnimator;
    public AudioSource audioSource;
    public AudioClip closeScrollSound;
    public Button button;
    public TextMeshProUGUI textComponent;
    public Image buttonImage;

    private string originalText;
    public Action onCompleteAction;

    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    private void Start()
    {
        if (textComponent != null)
        {
            originalText = textComponent.text;
        }

        button.onClick.AddListener(OnButtonClicked);

        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == button.gameObject)
        {
            if (buttonImage != null && buttonImage.color != selectedColor)
            {
                buttonImage.color = selectedColor;
            }
        }
        else
        {
            if (buttonImage != null && buttonImage.color != normalColor)
            {
                buttonImage.color = normalColor;
            }
        }
    }

    private void OnButtonClicked()
    {
        scrollAnimator.SetTrigger("Close");
        audioSource.PlayOneShot(closeScrollSound);

        if (textComponent != null)
        {
            textComponent.gameObject.SetActive(false);
        }

        StartCoroutine(WaitAndExecuteAction());
    }

    private IEnumerator WaitAndExecuteAction()
    {
        float waitTime = Mathf.Max(scrollAnimator.GetCurrentAnimatorStateInfo(0).length, closeScrollSound.length);
        yield return new WaitForSeconds(waitTime);

        onCompleteAction?.Invoke();
    }

    public void ResetScroll()
    {
        scrollAnimator.Play("Idle", 0, 0f);

        if (textComponent != null)
        {
            if (string.IsNullOrEmpty(originalText))
                originalText = textComponent.text;

            textComponent.text = originalText;
            textComponent.gameObject.SetActive(true);
        }

        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }
}
