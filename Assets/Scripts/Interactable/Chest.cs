using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;
    [SerializeField] private Animator chestAnimator;
    public bool Interact(Interactor interactor)
    {
        // Debug.Log("Opening chest");
        PlayerController playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("OpenChest");
            gameObject.layer = LayerMask.NameToLayer("Default");
            if (playerScript is not null) playerScript.isSlingshotActive = true;
        }
        else
        {
            Debug.Log("No animator");
        }
        return true;
    }
}
