using UnityEngine;
using System;
using System.Collections.Generic;
public class ResourcesManager : MonoBehaviour
{
    public static event Action<string, int> OnResourceAmountChanged;
    private Dictionary<string, int> resources = new();

    private void Start()
    {
        // Initialize resources with default values
        SetResourceAmount("wheat", 0);
        SetResourceAmount("bread", 0);
        SetResourceAmount("money", 0);
    }

    public void SetResourceAmount(string resourceName, int amount)
    {
        resources[resourceName] = amount;
        OnResourceAmountChanged?.Invoke(resourceName, amount);
    }
    public void AddResource(string resourceName, int amount)
    {
        var currentAmount = resources.ContainsKey(resourceName) ? resources[resourceName] : 0;
        SetResourceAmount(resourceName, currentAmount + amount);
    }

    public bool IsAvailableToCreateFood(IngredientAmount[] ingredientAmounts)
    {
        foreach (var ingredientAmount in ingredientAmounts)
        {
            var id = ingredientAmount.ingredient.GetId();
            var requiredAmount = ingredientAmount.amount;
            var currentAmount = resources.ContainsKey(id) ? resources[id] : 0;
            if (currentAmount < requiredAmount)
            {
                return false;
            }
        }
        return true;
    }
    public void ConsumeResources(IngredientAmount[] ingredientAmounts)
    {
        foreach (var ingredientAmount in ingredientAmounts)
        {
            var id = ingredientAmount.ingredient.GetId();
            var requiredAmount = ingredientAmount.amount;
            AddResource(id, -requiredAmount);
        }
    }
    public bool IsAvailableFood(FoodAmount[] foodAmounts)
    {
        foreach (var foodAmount in foodAmounts)
        {
            var id = foodAmount.foodId;
            var requiredAmount = foodAmount.amount;
            var currentAmount = resources.ContainsKey(id) ? resources[id] : 0;
            if (currentAmount < requiredAmount)
            {
                return false;
            }
        }
        return true;
    }
    public void ConsumeFood(FoodAmount[] foodAmounts)
    {
        foreach (var foodAmount in foodAmounts)
        {
            var id = foodAmount.foodId;
            var amount = foodAmount.amount;
            AddResource(id, -amount);
        }
    }

}
