using System.Threading;
using Cysharp.Threading.Tasks;

public class DinerEatFood : BaseWorker
{
    private float eatDuration = 5f;
    private DiningTable diningTable;

    public DinerEatFood(DiningTable diningTable, float eatDuration)
    {
        this.eatDuration = eatDuration;
        this.diningTable = diningTable;
    }
    
    public override string JobName()
    {
        return "DinerEatFood";
    }
    
    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        character.ChatPanelComponent.ShowChat("Yummy!");
        await UniTask.WaitForSeconds(eatDuration, cancellationToken: cancellationToken);
        EventBus.Publish(new AddResourceEvent(InventoryItemId.MONEY, 5));
        diningTable.VacateSeat(character.transform);
        character.EnqueueJob(new Pedestrian());
        character.ChatPanelComponent.HideChat();
    }
}
