using System.Collections;
using UnityEngine;
using static EntityConstant;
using FoodType = ResourceConstant.Food.FoodType;
using Building = EntityConstant.Building;
using System.Collections.Generic;
public class Server : BaseWorker
{
    private float workDuration = 2f;
    private bool isWorking = false;
    public Server(float workDuration) : base()
    {
        this.workDuration = workDuration;
        FoodOrderManager.OnFoodOrderAdded += HandleFoodOrderAdded;

    }
    ~Server()
    {
        FoodOrderManager.OnFoodOrderAdded -= HandleFoodOrderAdded;
    }
    public override string JobName()
    {
        return "Server";
    }

    private void HandleFoodOrderAdded()
    {
        if (isWorking) return;
        doable.DoJobAsync(DoJobAsync);
    }


    public override IEnumerator DoJobAsync()
    {
        var entityManager = GameManager.Instance.GamePlay.EntityManager;
        var resourcesManager = GameManager.Instance.GamePlay.ResourcesManager;
        var foodOrderManager = GameManager.Instance.GamePlay.FoodOrderManager;
        var servingTable = entityManager.GetActiveEntity(Building.SERVING_TABLE);
        var orderTable = entityManager.GetActiveEntity(Building.ORDER_TABLE);
        var order = foodOrderManager.GetOldestFoodOrder();
        if (order == null) yield break;
        //! race conditions
        isWorking = true;
        if (resourcesManager.IsAvailableFood(order.foodAmounts))
        {
            foodOrderManager.RemoveFoodOrder();
            yield return movement.Move(servingTable.transform.position);
            resourcesManager.ConsumeFood(order.foodAmounts);
            yield return movement.Move(order.diningTable.transform.position);
            order.diner.EatFood();
            var orderLeft = foodOrderManager.GetOldestFoodOrder();
            if (orderLeft != null)
            {
                isWorking = false;
                HandleFoodOrderAdded();
            }
            else
            {
                yield return movement.Move(orderTable.transform.position);
                isWorking = false;
            }
        }
        else
        {
            yield return movement.Move(orderTable.transform.position);
            var orderLeft = foodOrderManager.GetOldestFoodOrder();
            if (orderLeft != null)
            {
                HandleFoodOrderAdded();
            }
            isWorking = false;

        }

    }

}
