using System.Linq;
using UnityEngine;

public class Food : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLife life = other.GetComponent<PlayerLife>();
            if (life != null && FoodValues.Instance != null)
            {
                string foodName = gameObject.name.Replace("(Clone)", "").Trim();

                var match = FoodValues.Instance.listFoodValues
                    .FirstOrDefault(foodValue => foodValue.food != null && foodValue.food.name == foodName);

                if (match.food != null)
                {
                    life.ChangeCurrentLife(match.recoverValue);
                    Debug.Log($"Tanuki ha recuperado {match.recoverValue} hambre de {foodName}!");
                }
                else
                {
                    Debug.LogWarning($"La comida '{foodName}' no se encuentra en la lista de comidas.");
                }

                Destroy(gameObject);
            }
        }
    }
}
