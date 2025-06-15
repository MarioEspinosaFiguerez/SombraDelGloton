using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [Header("Pendulum Movement")]
    [SerializeField] private float minRangeRotation = 0;
    [SerializeField] private float maxRangeRotation = 0;
    [SerializeField] private float rotationSpeed = 0;
    // float x = 0.7f;
    // private BoxCollider[] colliders;

    [Header("Damage")]
    [SerializeField] private float damageAmount = 25f;
    [SerializeField] private string targetTag = "Player";
    public float currentRotation;
    private int direction = 1;
    [SerializeField] private bool IsHorizontal = true;
    // Start is called before the first frame update
    void Start()
    {        
        currentRotation = (minRangeRotation + maxRangeRotation) / 2f;
    }

    // Update is called once per frame
    void Update()
    {       
            currentRotation += rotationSpeed * Time.deltaTime * direction;
            if(currentRotation >= maxRangeRotation)
            {
                currentRotation = maxRangeRotation;
                direction = -1;
            }
            else if(currentRotation <= minRangeRotation)
            {
                currentRotation = minRangeRotation;
                direction = 1;
            }
            if(IsHorizontal)
            {
                transform.localRotation = Quaternion.Euler(currentRotation, 90f, 0f);
            }
            else 
            {
                transform.localRotation = Quaternion.Euler(currentRotation, 180f, 0f);  
            }
    }

    void OnTriggerEnter(Collider collider)
    {
        // Debug.Log(other);
        Debug.Log(1234);
        if(collider.CompareTag(targetTag))
        {
            PlayerLife player = collider.GetComponent<PlayerLife>();
            if (player == null)
            {
                player = collider.GetComponentInParent<PlayerLife>();
            }
            if (player != null)
            {
                player.ChangeCurrentLife(-damageAmount);
            }
        }
    }
}
