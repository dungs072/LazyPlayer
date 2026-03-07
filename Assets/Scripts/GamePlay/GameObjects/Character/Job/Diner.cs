using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static EntityConstant;

public class Diner : BaseWorker
{
    private EntityManager entityManager;
    private ResourcesManager resourcesManager;
    private FoodOrderManager foodOrderManager;
    private TableOrderManager tableOrderManager;
    private JobFactory jobFactory;

    public Diner(EntityManager entityManager, ResourcesManager resourcesManager, FoodOrderManager foodOrderManager, TableOrderManager tableOrderManager, JobFactory jobFactory)
    {
        this.entityManager = entityManager;
        this.resourcesManager = resourcesManager;
        this.foodOrderManager = foodOrderManager;
        this.tableOrderManager = tableOrderManager;
        this.jobFactory = jobFactory;
    }
    
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

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        diningTable = entityManager.GetAvailableDiningTable(Building.DINING_TABLE);
        if (diningTable == null)
        {
            tableOrderManager.AddTableOrder(new TableOrder() { diner = this });
            return;
        }
        var targetPos = diningTable.GetAvailableSeat();
        diningTable.OccupySeat(transform);
        if (targetPos == null)
        {
            Debug.Log("No available seat found for diner");
        }
        else
        {
            await movement.Move(cancellationToken, targetPos.Value);
               
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
    private async UniTask EatFoodAsync(CancellationToken cancellationToken)
    {
        chatPanel.ShowChat("Yummy!");
        await UniTask.WaitForSeconds(eatDuration, cancellationToken: cancellationToken);
       
        resourcesManager.AddResource("money", 5);
        var pedestrian = jobFactory.CreatePedestrian();
        switchableJob.SetJob(pedestrian);
        diningTable.VacateSeat(transform);
        doable.DoJobAsync(pedestrian.DoJobAsync);
        chatPanel.HideChat();
    }
}
