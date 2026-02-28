using System.Collections;
using UnityEngine;
using static EntityConstant;

public class Diner : BaseWorker
{
    private float eatDuration = 5f;
    private DiningTable diningTable;
    public override string JobName()
    {
        return "Diner";
    }
    public void DoJob()
    {
        doable.DoJobAsync(DoJobAsync);
    }

    public override IEnumerator DoJobAsync()
    {
        var entityManager = GameManager.Instance.GamePlay.EntityManager;
        var foodOrderManager = GameManager.Instance.GamePlay.FoodOrderManager;
        var tableOrderManager = GameManager.Instance.GamePlay.TableOrderManager;
        diningTable = entityManager.GetAvailableDiningTable(Building.DINING_TABLE);
        if (diningTable == null)
        {
            tableOrderManager.AddTableOrder(new TableOrder() { diner = this });
            yield break;
        }
        var targetPos = diningTable.GetAvailableSeat();
        diningTable.OccupySeat(transform);
        if (targetPos == null)
        {
            Debug.Log("No available seat found for diner");
            yield break;
        }
        else
        {
            yield return movement.Move(targetPos.Value);
            chatPanel.ShowChat("x1 bread");
            foodOrderManager.AddFoodOrder(new FoodOrder()
            {
                diningTable = diningTable,
                diner = this,
                foodAmounts = new FoodAmount[]
                {
                    new() { foodId = "bread", amount = 1 },
                }
            });
        }
    }
    public void EatFood()
    {
        doable.DoJobAsync(EatFoodAsync);
    }
    private IEnumerator EatFoodAsync()
    {
        chatPanel.ShowChat("Yummy!");
        yield return new WaitForSeconds(eatDuration);
        var resourcesManager = GameManager.Instance.GamePlay.ResourcesManager;
        resourcesManager.AddResource("money", 5);
        var pedestrian = new Pedestrian();
        switchableJob.SetJob(pedestrian);
        diningTable.VacateSeat(transform);
        doable.DoJobAsync(pedestrian.DoJobAsync);
        chatPanel.HideChat();
    }
}
