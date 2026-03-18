using System;
using System.Collections.Generic;
using UnityEngine;
public class FoodAmount
{
    public InventoryItemId foodId;
    public int amount;
}
public class FoodOrder
{
    public FoodAmount[] foodAmounts;
    public DiningTable diningTable;
    public Character diner;
}


public class FoodOrderManager : MonoBehaviour
{
    public void Initialize1()
    {
        EventBus.Subscribe<AddFoodOrderEvent>(ev => AddFoodOrder(ev.foodOrder));
        EventBus.Subscribe<ReadyToServeFoodEvent>(ev => ReadyToServeFood());
        EventBus.Subscribe<RemoveFoodOrderEvent>(ev => RemoveFoodOrder());
        QueryBus.Subscribe<GetOldestFoodOrderQuery, FoodOrder>(query => GetOldestFoodOrder());
    }
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
