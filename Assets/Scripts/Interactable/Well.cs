using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Well : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;
    public bool Interact(Interactor interactor)
    {
        // throw new System.NotImplementedException();
        Debug.Log("Enter Well");
        GameManager.Instance.lifeBeforeSceneChanged = GameManager.Instance.playerLife.currentLife;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        return true;
    }
}
