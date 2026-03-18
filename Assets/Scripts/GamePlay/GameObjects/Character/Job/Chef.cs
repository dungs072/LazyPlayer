using System.Threading;
using Cysharp.Threading.Tasks;

public class Chef : BaseWorker
{
    private float workDuration = 5f;

    public Chef(float workDuration)
    {
        this.workDuration = workDuration;
    }

    public override string JobName()
    {
        return "Chef";
    }

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var breadRecipe = QueryBus.Query(new GetRecipeDataQuery(RecipeId.BREAD));
        var entityManager = character.entityManager;
        var canCook = QueryBus.Query(
            new IsAvailableToCreateFoodQuery(breadRecipe.GetIngredients())
        );
        if (canCook)
        {
            var kitchen = entityManager.FindActiveEntity(EntityId.KITCHEN);
            await character.MovementComponent.Move(cancellationToken, kitchen.transform.position);

            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);

            await EventBus.PublishAsync(new ConsumeResourceEvent(breadRecipe.GetIngredients()));
            var servingTable = entityManager.FindActiveEntity(EntityId.SERVING_TABLE);
            await character.MovementComponent.Move(
                cancellationToken,
                servingTable.transform.position
            );
            await EventBus.PublishAsync(new AddResourceEvent(InventoryItemId.BREAD, 1));
            EventBus.Publish(new ReadyToServeFoodEvent());
        }

        await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
        character.EnqueueJob(this);
    }
}
