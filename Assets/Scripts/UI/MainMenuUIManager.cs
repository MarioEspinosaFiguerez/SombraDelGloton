using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using System;
using UnityEngine.EventSystems;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject multiplayerMenu;
    public GameObject createSessionMenu;
    public GameObject joinSessionMenu;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValue;
    public TMP_InputField createSessionNameInput;
    public TMP_InputField createSessionPasswordInput;
    public TMP_InputField joinSessionNameInput;
    public TMP_InputField joinSessionPasswordInput;

    public ScrollButton[] scrollButtons;
    public Button MainMenuFirstButton;
    public Button OptionsFirstButton;
    public Button MultiplayerFirstButton;
    public Button CreateSessionFirstButton;
    public Button JoinSessionFirstButton;


    private void Start()
    {
        volumeSlider.value = 1f;
        volumeSlider.onValueChanged.AddListener(onVolumeChanged);
        UpdateVolumeValue(1f);
        EventSystem.current.SetSelectedGameObject(MainMenuFirstButton.gameObject);
    }

    private void UpdateVolumeValue(float value)
    {
        volumeValue.text = Mathf.RoundToInt(value * 100f) + "";
    }

    private void onVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume(value);
        UpdateVolumeValue(value);
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MainMenuFirstButton.gameObject);
        foreach (var button in scrollButtons)
        {
            button.ResetScroll();
        }
    }

    public void ShowOptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        multiplayerMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(OptionsFirstButton.gameObject);
        foreach (var button in scrollButtons)
        {
            button.ResetScroll();
        }
    }

    public void ShowMultiplayerMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
        createSessionMenu.SetActive(false);
        joinSessionMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MultiplayerFirstButton.gameObject);
        foreach (var button in scrollButtons)
        {
            button.ResetScroll();
        }
    }

    public void ShowCreateSessionMenu()
    {
        multiplayerMenu.SetActive(false);
        createSessionMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(CreateSessionFirstButton.gameObject);
        foreach (var button in scrollButtons)
        {
            button.ResetScroll();
        }
    }

    public void ShowJoinSessionMenu()
    {
        multiplayerMenu.SetActive(false);
        joinSessionMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(JoinSessionFirstButton.gameObject);
        foreach (var button in scrollButtons)
        {
            button.ResetScroll();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnClickCreateSession()
    {
        if (string.IsNullOrEmpty(createSessionNameInput.text) || string.IsNullOrEmpty(createSessionPasswordInput.text)) return;

        NetworkManager.Singleton.StartHost();
        NetworkSessionManager.Instance.SetSessionPassword(createSessionPasswordInput.text);
        NetworkSessionManager.Instance.RequestLoginServerRpc(
            createSessionPasswordInput.text,
            createSessionNameInput.text,
            NetworkManager.Singleton.LocalClientId
        );
    }
    
    public void OnClickJoinSession()
    {
        if (string.IsNullOrEmpty(joinSessionNameInput.text) || string.IsNullOrEmpty(joinSessionPasswordInput.text)) return;

        NetworkManager.Singleton.StartClient();
        StartCoroutine(WaitAndRequestLogin());
    }

    private IEnumerator WaitAndRequestLogin()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.IsConnectedClient);

        NetworkSessionManager.Instance.RequestLoginServerRpc(
            joinSessionPasswordInput.text,
            joinSessionNameInput.text,
            NetworkManager.Singleton.LocalClientId
        );
    }
}
