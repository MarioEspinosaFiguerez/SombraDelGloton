using UnityEngine;

public class KatanaTrigger : MonoBehaviour
{
    [SerializeField] private int katanaDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Tanuki golpeado por Hajime");
            other.GetComponent<PlayerLife>().ChangeCurrentLife(-katanaDamage);
        }
    }
}