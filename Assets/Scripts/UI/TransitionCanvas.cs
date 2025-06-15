using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionCanvas : MonoBehaviour
{
    public Image blackPanel;

    public static TransitionCanvas Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        blackPanel.gameObject.SetActive(true);
        StartCoroutine(FadeOut(blackPanel, 1f));
    }

    public IEnumerator FadeIn(Image img, float duration)
    {
        img.gameObject.SetActive(true);
        Color color = img.color;
        float startAlpha = 0f;
        float endAlpha = 1f;
        float elapsed = 0f;

        color.a = startAlpha;
        img.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0, 1, t);

            color.a = Mathf.Lerp(startAlpha, endAlpha, smoothT);
            img.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeOut(Image img, float duration)
    {
        Color color = img.color;
        float startAlpha = 1f;
        float endAlpha = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0, 1, t);

            color.a = Mathf.Lerp(startAlpha, endAlpha, smoothT);
            img.color = color;
            yield return null;
        }

        img.gameObject.SetActive(false);
    }

    public void CallNextScene()
    {
        StartCoroutine(NextScene());
    }

    private IEnumerator NextScene()
    {
        blackPanel.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn(blackPanel, 1f));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
