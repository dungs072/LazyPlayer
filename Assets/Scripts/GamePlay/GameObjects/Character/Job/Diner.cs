using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;
using static EntityConstant;

public class Diner : BaseWorker
{
    private TableOrderManager tableOrderManager;
    private JobFactory jobFactory;

    public Diner(TableOrderManager tableOrderManager, JobFactory jobFactory)
    {
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
        diningTable = QueryBus.Query<GetActiveEntityQuery, DiningTable>(
            new GetActiveEntityQuery(Building.DINING_TABLE)
        );
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
            EventBus.Publish(new AddFoodOrderEvent(
                new FoodOrder()
                {
                    diningTable = diningTable,
                    diner = this,
                    foodAmounts = new FoodAmount[]
                    {
                        new() { foodId = "bread", amount = 1 },
                    },
                }
            ));
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
        EventBus.Publish(new AddResourceEvent("money", 5));
        var pedestrian = jobFactory.CreatePedestrian();
        switchableJob.SetJob(pedestrian);
        diningTable.VacateSeat(transform);
        doable.DoJobAsync(pedestrian.DoJobAsync);
        chatPanel.HideChat();
    }
}
