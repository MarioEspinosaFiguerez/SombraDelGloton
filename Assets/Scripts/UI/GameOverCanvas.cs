using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverCanvas : MonoBehaviour
{
    public Image blackPanel;
    public GameObject scrollContainer;
    public Image scrollHead;
    public Image scrollBody;
    public TextMeshProUGUI textBlock;
    private float scrollHeadStartValue = 1070f;
    private float scrollContainerStartValue = -350f;
    private Slider scrollBodySlider;

    public GameObject buttonContainer;
    public Button retryButton;
    public Button exitButton;

    private bool isRunningCoroutine = false;
    private bool hasShownGameOver = false;

    public static GameOverCanvas Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        scrollBodySlider = scrollBody.GetComponent<Slider>();
        blackPanel.gameObject.SetActive(true);
        textBlock.text = "";
        scrollBodySlider.value = 0;

        blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, 0f);
    }

    public void CallGameOverMenu()
    {
        if (hasShownGameOver) return;
        hasShownGameOver = true;
        StartCoroutine(GameOverMenu());
    }

    public IEnumerator GameOverMenu()
    {
        yield return StartCoroutine(FadeIn(blackPanel, 3f));
        yield return StartCoroutine(MoveUpContainer(scrollContainer, 0.75f));
        yield return StartCoroutine(OpenScroll(2f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(TypeText("GAME OVER :(", 0.075f));

        StartCoroutine(MoveUpContainer(buttonContainer, 0.75f));
    }

    public void CallReturnToMenu()
    {
        if (!isRunningCoroutine)
            StartCoroutine(ReturnToMenuCoroutine());
    }

    public void CallTryAgain()
    {
        if (!isRunningCoroutine)
            StartCoroutine(TryAgainCoroutine());
    }

    private IEnumerator ReturnToMenuCoroutine()
    {
        isRunningCoroutine = true;
        yield return StartCoroutine(TypeText("", 0.075f));
        yield return StartCoroutine(CloseScroll(0.75f));
        yield return StartCoroutine(MoveDownContainer(scrollContainer, 0.75f));
        yield return StartCoroutine(MoveDownContainer(buttonContainer, 0.75f));
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(1); // 1 = menú
        isRunningCoroutine = false;
    }
    private IEnumerator TryAgainCoroutine()
    {
        isRunningCoroutine = true;
        yield return StartCoroutine(TypeText("", 0.075f));
        yield return StartCoroutine(CloseScroll(0.75f));
        yield return StartCoroutine(MoveDownContainer(scrollContainer, 0.75f));
        yield return StartCoroutine(MoveDownContainer(buttonContainer, 0.75f));
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        isRunningCoroutine = false;
    }

    public IEnumerator TypeText(string fullText, float delay)
    {
        textBlock.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            textBlock.text += fullText[i];
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator FadeIn(Image img, float duration)
    {
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

    public IEnumerator OpenScroll(float duration)
    {
        RectTransform rt = scrollHead.rectTransform;
        float startX = scrollHeadStartValue;
        float targetX = 0f;
        Vector3 startPos = new Vector3(startX, rt.localPosition.y, rt.localPosition.z);
        rt.localPosition = startPos;

        float startValue = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // Mover scrollHead
            float newX = Mathf.Lerp(startX, targetX, smoothT);
            rt.localPosition = new Vector3(newX, rt.localPosition.y, rt.localPosition.z);

            // Cambiar valor del slider
            scrollBodySlider.value = Mathf.Lerp(startValue, 1f, smoothT);

            yield return null;
        }
    }

    public IEnumerator CloseScroll(float duration)
    {
        RectTransform rt = scrollHead.rectTransform;
        float startX = 0f;
        float targetX = scrollHeadStartValue;
        Vector3 startPos = new Vector3(startX, rt.localPosition.y, rt.localPosition.z);
        rt.localPosition = startPos;

        float startValue = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // Mover scrollHead hacia la derecha
            float newX = Mathf.Lerp(startX, targetX, smoothT);
            rt.localPosition = new Vector3(newX, rt.localPosition.y, rt.localPosition.z);

            // Reducir el valor del slider
            scrollBodySlider.value = Mathf.Lerp(startValue, 0f, smoothT);

            yield return null;
        }
    }

    public IEnumerator MoveUpContainer(GameObject container, float duration)
    {
        RectTransform rt = container.GetComponent<RectTransform>();

        Vector3 startPos = rt.localPosition;
        Vector3 targetPos = new Vector3(startPos.x, 0, startPos.z);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0, 1, t);

            rt.localPosition = Vector3.Lerp(startPos, targetPos, smoothT);

            yield return null;
        }
    }

    public IEnumerator MoveDownContainer(GameObject container, float duration)
    {
        RectTransform rt = container.GetComponent<RectTransform>();

        Vector3 startPos = rt.localPosition;
        Vector3 targetPos = new Vector3(startPos.x, -880f, startPos.z);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0, 1, t);

            rt.localPosition = Vector3.Lerp(startPos, targetPos, smoothT);

            yield return null;
        }
    }
}
