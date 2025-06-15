using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CinematicManager : MonoBehaviour
{
    public Image blackPanel;

    public GameObject scrollContainer;
    public Image scrollHead;
    public Image scrollBody;
    public TextMeshProUGUI textBlock;
    private float scrollHeadStartValue = 1070f;
    private float scrollContainerStartValue = -350f;
    private Slider scrollBodySlider;

    public GameObject introImageContainer;
    private Image[] introImages;
    public string[] introTexts;
    public float imageOffset = 900f;


    void Start()
    {
        introImages = introImageContainer.GetComponentsInChildren<Image>();     //Se reversa el orden
        Array.Reverse(introImages);
        scrollBodySlider = scrollBody.GetComponent<Slider>();
        blackPanel.gameObject.SetActive(true);
        textBlock.text = "";
        scrollBodySlider.value = 0;

        StartCoroutine(IntroSequence());
    }


    //----------------------------------------------------------------------------------------------

    // Función hecha para seguir una sequencia, si el número de los arrays de imagen y texto no coincide puede fallar.
    IEnumerator IntroSequence()
    {
        //FadeIn de la pantalla, el scroll aparece y empieza la intro.
        yield return StartCoroutine(FadeOut(blackPanel, 3f));
        yield return StartCoroutine(MoveUpScrollContainer(1f));
        yield return StartCoroutine(OpenScroll(2f));
        yield return new WaitForSeconds(0.75f);

        yield return StartCoroutine(TypeText(introTexts[0], 0.075f));            //Introduccion
        yield return StartCoroutine(MoveImageLeft(introImages[0], 0.5f));
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(TypeText(introTexts[1], 0.075f));            //Quemado
        yield return StartCoroutine(MoveImageLeft(introImages[1], 1f));
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(TypeText(introTexts[2], 0.075f));            //Akechi, frente
        yield return StartCoroutine(MoveImageLeft(introImages[2], 1f));
        yield return new WaitForSeconds(1f);

        //yield return StartCoroutine(MoveImageLeft(introImages[3], 1f));          //Capturado
        yield return StartCoroutine(TypeText(introTexts[3], 0.075f));
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(MoveImageLeft(introImages[3], 1.5f));          //Escapar
        yield return StartCoroutine(TypeText(introTexts[4], 0.075f));
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(MoveImageLeft(introImages[4], 1.5f));          //Festejando, hay dos lineas de texto asociado
        yield return StartCoroutine(TypeText(introTexts[5], 0.075f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(TypeText(introTexts[6], 0.075f));
        yield return new WaitForSeconds(0.75f);

        yield return StartCoroutine(MoveImageLeft(introImages[5], 1.5f));          //Sombra castillo
        yield return StartCoroutine(TypeText(introTexts[7], 0.075f));
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(MoveImageLeft(introImages[6], 1.5f));          //Sombra torii
        yield return StartCoroutine(TypeText(introTexts[8], 0.075f));
        yield return new WaitForSeconds(1f);

        StartCoroutine(MoveImageLeft(introImages[7], 7f));                       //Tanuki?
        yield return StartCoroutine(TypeText(introTexts[9], 0.075f));
        yield return new WaitForSeconds(.5f);
        yield return StartCoroutine(TypeText(introTexts[10], 0.075f));
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(TypeText(introTexts[11], 0.075f));
        yield return new WaitForSeconds(0.6f);

        yield return StartCoroutine(MoveImageLeft(introImages[8], 1.5f));          //Tanuki corriendo
        yield return StartCoroutine(TypeText(introTexts[12], 0.075f));
        yield return new WaitForSeconds(2f);

        StartCoroutine(TypeText("", 0.075f));
        StartCoroutine(FadeIn(blackPanel, 2f));
        yield return StartCoroutine(CloseScroll(1f));
        yield return StartCoroutine(MoveDownScrollContainer(.75f));

        //Linea para pasar a la siguiente escena, modificad a libertad.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
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

    public IEnumerator MoveImageLeft(Image img, float duration)
    {
        RectTransform rt = img.rectTransform;
        Vector3 startPos = rt.anchoredPosition;
        Vector3 targetPos = startPos + Vector3.left * imageOffset;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            rt.anchoredPosition = Vector3.Lerp(startPos, targetPos, smoothT);

            yield return null;
        }
    }

    public IEnumerator MoveUpScrollContainer(float duration)
    {
        RectTransform rt = scrollContainer.GetComponent<RectTransform>();

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

    public IEnumerator MoveDownScrollContainer(float duration)
    {
        RectTransform rt = scrollContainer.GetComponent<RectTransform>();

        Vector3 startPos = rt.localPosition;
        Vector3 targetPos = new Vector3(startPos.x, scrollContainerStartValue, startPos.z);
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
    }
}
