using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodChest : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;

    [SerializeField] private Animator chestAnimator;
    [SerializeField] private List<GameObject> foodPrefabs;
    [SerializeField] private float dropDistance = 1.5f;
    [SerializeField] private float dropHeight = 0.5f;
    [SerializeField] private float chestOpenAnimationDuration = 1.1f; 

    private Transform playerTransform;

    public bool Interact(Interactor interactor)
    {
        PlayerController playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("OpenChest");
            gameObject.layer = LayerMask.NameToLayer("Default");

            if (playerScript != null)
            {
                playerTransform = playerScript.transform;
                StartCoroutine(DropFoodAfterDelay(chestOpenAnimationDuration));
            }
        }
        else
        {
            Debug.Log("No animator");
        }

        return true;
    }

    private IEnumerator DropFoodAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DropRandomFood();
    }

    private void DropRandomFood()
    {
        if (foodPrefabs.Count == 0)
        {
            Debug.LogWarning("No hay comida asssignada al cofre");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("No se encontró player");
            return;
        }

        int randomIndex = Random.Range(0, foodPrefabs.Count);
        GameObject selectedFood = foodPrefabs[randomIndex];

        Vector3 dropPosition = playerTransform.position + playerTransform.forward * dropDistance;
        dropPosition.y += dropHeight;

        Instantiate(selectedFood, dropPosition, Quaternion.identity);
    }
}
