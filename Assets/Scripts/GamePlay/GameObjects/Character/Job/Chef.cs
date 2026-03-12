using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Building = EntityConstant.Building;

public class Chef : BaseWorker
{
    private float workDuration = 5f;

    public Chef(float workDuration)
        : base()
    {
        this.workDuration = workDuration;
    }

    public override string JobName()
    {
        return "Chef";
    }

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var breadRecipe = QueryBus.Query(new GetRecipeDataQuery("bread"));
        var canCook = QueryBus.Query(
            new IsAvailableToCreateFoodQuery(breadRecipe.GetIngredients())
        );
        if (canCook)
        {
            var kitchen = QueryBus.Query(new GetActiveEntityQuery(Building.KITCHEN));
            await movement.Move(cancellationToken, kitchen.transform.position);

            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);

            await EventBus.PublishAsync(new ConsumeResourceEvent(breadRecipe.GetIngredients()));
            var servingTable = QueryBus.Query(new GetActiveEntityQuery(Building.SERVING_TABLE));
            await movement.Move(cancellationToken, servingTable.transform.position);
            await EventBus.PublishAsync(new AddResourceEvent("bread", 1));
            EventBus.Publish(new ReadyToServeFoodEvent());
        }

        await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
    }

    private async UniTask DoNothing(CancellationToken cancellationToken)
    {
        var randomPos = GetRandomPositionInScreen();
        await movement.Move(cancellationToken, randomPos);

        await UniTask.WaitForSeconds(1.5f, cancellationToken: cancellationToken);
    }

    private Vector3 GetRandomPositionInScreen()
    {
        Camera cam = Camera.main;

        float x = Random.Range(0f, Screen.width);
        float y = Random.Range(0f, Screen.height);
        float z = 10f;

        return cam.ScreenToWorldPoint(new Vector3(x, y, z));
    }
}
