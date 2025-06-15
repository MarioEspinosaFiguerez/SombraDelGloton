using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public MainMenuUIManager menuManager;

    public ScrollButton playButton;
    public ScrollButton optionsButton;
    public ScrollButton multiplayerButton;
    public ScrollButton exitButton;
    public ScrollButton backButtonOptions;
    public ScrollButton backButtonMultiplayer;
    public ScrollButton joinSessionButton;
    public ScrollButton createSessionButton;
    public ScrollButton backToMultiplayerButton;
    public ScrollButton CreateSessionButton;
    public ScrollButton JoinSessionButton;
    public ScrollButton backToMultiplayerButton2;

    private void Start()
    {
        playButton.onCompleteAction = () => menuManager.StartGame();
        optionsButton.onCompleteAction = () => menuManager.ShowOptionsMenu();
        multiplayerButton.onCompleteAction = () => menuManager.ShowMultiplayerMenu();
        exitButton.onCompleteAction = () => menuManager.ExitGame();
        backButtonOptions.onCompleteAction = () => menuManager.ShowMainMenu();
        backButtonMultiplayer.onCompleteAction = () => menuManager.ShowMainMenu();
        joinSessionButton.onCompleteAction = () => menuManager.ShowJoinSessionMenu();
        createSessionButton.onCompleteAction = () => menuManager.ShowCreateSessionMenu();
        backToMultiplayerButton.onCompleteAction = () => menuManager.ShowMultiplayerMenu();
        CreateSessionButton.onCompleteAction = () => menuManager.OnClickCreateSession();
        JoinSessionButton.onCompleteAction = () => menuManager.OnClickJoinSession();
        backToMultiplayerButton2.onCompleteAction = () => menuManager.ShowMultiplayerMenu();
        
    }
}
