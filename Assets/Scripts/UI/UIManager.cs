using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public MonoBehaviour[] scripts;
    private bool isPaused = false;

    [SerializeField] private RectTransform imageToUnroll;
    [SerializeField] private float unrollDuration = 1f;

    private float originalHeight;
    private float elapsedTime = 0f;
    private bool animateRoll = false;
    private bool isUnrolling = true;
    // public bool showButtons = false;

    [SerializeField] GameObject[] buttons;

    void Start()
    {
        if (imageToUnroll != null)
        {
            originalHeight = imageToUnroll.rect.height;
            imageToUnroll.sizeDelta = new Vector2(imageToUnroll.sizeDelta.x, 0f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (animateRoll && imageToUnroll != null)
        {
            if (elapsedTime < unrollDuration)
            {
                elapsedTime += Time.unscaledDeltaTime; 
                float progress = Mathf.Clamp01(elapsedTime / unrollDuration);
                float newHeight = isUnrolling
                    ? Mathf.Lerp(0f, originalHeight, progress)
                    : Mathf.Lerp(originalHeight, 0f, progress);

                imageToUnroll.sizeDelta = new Vector2(imageToUnroll.sizeDelta.x, newHeight);
            }
            else
            {
                animateRoll = false;

                if (isUnrolling)
                {
                    ToggleButtons(true);
                }
                else
                {
                    pauseMenuUI.SetActive(false);
                    ResumeGame();
                }
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        animateRoll = true;
        elapsedTime = 0f;

        if (isPaused)
        {
            isUnrolling = true;
            pauseMenuUI.SetActive(true); 
            imageToUnroll.sizeDelta = new Vector2(imageToUnroll.sizeDelta.x, 0f); 
            ToggleButtons(false);
            PauseGame();
        }
        else
        {
            isUnrolling = false;
            ToggleButtons(false);
            // ResumeGame(); 
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        ToggleScripts(false);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        ToggleScripts(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ToggleScripts(bool value)
    {
        foreach (var script in scripts)
        {
            script.enabled = value;
        }
    }

    private void ToggleButtons(bool visible)
    {
        foreach (var button in buttons)
        {
            button.SetActive(visible);
        }
    }
}
