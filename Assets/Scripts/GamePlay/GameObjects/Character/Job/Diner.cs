using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor.Search;
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
        character.DoJobAsync(DoJobAsync);
    }

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        diningTable = QueryBus.Query<GetActiveEntityQuery, Entity>(
            new GetActiveEntityQuery(Building.DINING_TABLE)
        ) as DiningTable;
        if (diningTable == null)
        {
            EventBus.Publish(new AddTableOrderEvent(new TableOrder() { diner = this}));
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
            await movementComponent.Move(cancellationToken, targetPos.Value);

            chatPanelComponent.ShowChat("x1 bread");
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
        character.DoJobAsync(EatFoodAsync);
    }

    private async UniTask EatFoodAsync(CancellationToken cancellationToken)
    {
        chatPanelComponent.ShowChat("Yummy!");
        await UniTask.WaitForSeconds(eatDuration, cancellationToken: cancellationToken);
        EventBus.Publish(new AddResourceEvent("money", 5));
        var pedestrian = new Pedestrian(); 
        character.SetJob(pedestrian);
        diningTable.VacateSeat(transform);
        character.DoJobAsync(pedestrian.DoJobAsync);
        chatPanelComponent.HideChat();
    }
}
