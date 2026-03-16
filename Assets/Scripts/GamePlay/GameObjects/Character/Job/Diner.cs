using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static EntityConstant;

public class Diner : BaseWorker
{
    private DiningTable diningTable;

    public override string JobName()
    {
        return "Diner";
    }

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var entityManager = character.entityManager;
        diningTable = entityManager.FindActiveEntity<DiningTable>(EntityId.DINING_TABLE);
        if (diningTable == null)
        {
            EventBus.Publish(new AddTableOrderEvent(new TableOrder() { diner = character }));
            return;
        }
        var targetPos = diningTable.GetAvailableSeat();
        diningTable.OccupySeat(character.transform);
        if (targetPos == null)
        {
            Debug.Log("No available seat found for diner");
        }
        else
        {
            await character.MovementComponent.Move(cancellationToken, targetPos.Value);

            character.ChatPanelComponent.ShowChat("x1 bread");
            EventBus.Publish(
                new AddFoodOrderEvent(
                    new FoodOrder()
                    {
                        diningTable = diningTable,
                        diner = character,
                        foodAmounts = new FoodAmount[]
                        {
                            new() { foodId = "bread", amount = 1 },
                        },
                    }
                )
            );
        }
    }
}
