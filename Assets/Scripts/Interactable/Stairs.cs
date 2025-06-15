using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stairs : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;
    public bool Interact(Interactor interactor)
    {
        // throw new System.NotImplementedException();
        GameManager.Instance.lifeBeforeSceneChanged = GameManager.Instance.playerLife.currentLife;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        TransitionCanvas.Instance.CallNextScene();
        return true;
    }
}
