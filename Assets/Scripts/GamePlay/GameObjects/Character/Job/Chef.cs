using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Building = EntityConstant.Building;
public class Chef : BaseWorker
{
    private float workDuration = 5f;
    public Chef(float workDuration) : base()
    {
        this.workDuration = workDuration;
    }
    public override string JobName()
    {
        return "Chef";
    }

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var entityManager = GameManager.Instance.GamePlay.EntityManager;
        var resourcesManager = GameManager.Instance.GamePlay.ResourcesManager;
        var foodDictionary = GameManager.Instance.GamePlay.FoodDictionary;
        var foodOrderManager = GameManager.Instance.GamePlay.FoodOrderManager;
        var breadRecipe = foodDictionary.GetRecipeData("bread");
        var canCook = resourcesManager.IsAvailableToCreateFood(breadRecipe.GetIngredients());
        if (canCook)
        {
            var kitchen = entityManager.GetActiveEntity(Building.KITCHEN);
            await movement.Move(cancellationToken, kitchen.transform.position);
               
            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);
               
            resourcesManager.ConsumeResources(breadRecipe.GetIngredients());
            var servingTable = entityManager.GetActiveEntity(Building.SERVING_TABLE);
            await movement.Move(cancellationToken, servingTable.transform.position);
            resourcesManager.AddResource("bread", 1);
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
    public override void FinishCurrentStep()
    {
        currentStepIndex++;
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
