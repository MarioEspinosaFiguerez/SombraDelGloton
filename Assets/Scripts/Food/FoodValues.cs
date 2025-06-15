using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FoodValuesDic
{
    public GameObject food;
    public float recoverValue;

    public FoodValuesDic(GameObject food, float recoverValue) { this.food = food; this.recoverValue = recoverValue; }
}

public class FoodValues : MonoBehaviour
{
    public static FoodValues Instance;

    public List<FoodValuesDic> listFoodValues;

    private void Awake()
    {
        if (Instance is not null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}