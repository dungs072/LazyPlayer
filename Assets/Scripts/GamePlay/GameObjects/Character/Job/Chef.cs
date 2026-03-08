using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Building = EntityConstant.Building;
public class Chef : BaseWorker
{
    private FoodOrderManager foodOrderManager;
    
    private float workDuration = 5f;
    public Chef(float workDuration, FoodOrderManager foodOrderManager) : base()
    {
        this.workDuration = workDuration;
        this.foodOrderManager = foodOrderManager;
    }
    public override string JobName()
    {
        return "Chef";
    }

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var breadRecipe = QueryBus.Query<GetRecipeDataQuery, RecipeData>(new GetRecipeDataQuery("bread"));
        var canCook = QueryBus.Query<IsAvailableToCreateFoodQuery, bool>(new IsAvailableToCreateFoodQuery(breadRecipe.GetIngredients()));
        if (canCook)
        {
            var kitchen = QueryBus.Query<GetActiveEntityQuery, Entity>(new GetActiveEntityQuery(Building.KITCHEN));
            await movement.Move(cancellationToken, kitchen.transform.position);
               
            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);
               
            await EventBus.PublishAsync(new ConsumeResourceEvent(breadRecipe.GetIngredients()));
            var servingTable = QueryBus.Query<GetActiveEntityQuery, Entity>(new GetActiveEntityQuery(Building.SERVING_TABLE));
            await movement.Move(cancellationToken, servingTable.transform.position);
            await EventBus.PublishAsync(new AddResourceEvent("bread", 1));
            foodOrderManager.ReadyToServeFood();
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
