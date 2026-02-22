using System;
using System.Collections.Generic;
using UnityEngine;
public class FoodAmount
{
    public string foodId;
    public int amount;
}
public class FoodOrder
{
    public FoodAmount[] foodAmounts;
    public DiningTable diningTable;
    public Diner diner;
}


public class FoodOrderManager : MonoBehaviour
{
    public static event Action OnFoodOrderAdded;
    private Queue<FoodOrder> foodOrders = new();

    public void AddFoodOrder(FoodOrder foodOrder)
    {
        foodOrders.Enqueue(foodOrder);
        OnFoodOrderAdded?.Invoke();
    }
    public void ReadyToServeFood()
    {
        if (foodOrders.Count == 0) return;
        OnFoodOrderAdded?.Invoke();
    }

    public FoodOrder GetOldestFoodOrder()
    {
        if (foodOrders.Count > 0)
        {
            return foodOrders.Peek();
        }
        return null;
    }
    public void RemoveFoodOrder()
    {
        if (foodOrders.Count > 0)
        {
            foodOrders.Dequeue();
        }
    }
}
