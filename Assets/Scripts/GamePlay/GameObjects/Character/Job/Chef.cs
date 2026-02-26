using System.Collections;
using UnityEngine;
using static EntityConstant;
using FoodType = ResourceConstant.Food.FoodType;
using Building = EntityConstant.Building;
public class Chef : BaseWorker
{
    private float workDuration = 5f;
    public Chef(float workDuration) : base()
    {
        this.workDuration = workDuration;
    }

    public override IEnumerator DoJobAsync()
    {
        var entityManager = GameManager.Instance.EntityManager;
        var resourcesManager = GameManager.Instance.ResourcesManager;
        var foodDictionary = GameManager.Instance.FoodDictionary;
        var foodOrderManager = GameManager.Instance.FoodOrderManager;
        var breadRecipe = foodDictionary.GetRecipeData("bread");
        var canCook = resourcesManager.IsAvailableToCreateFood(breadRecipe.GetIngredients());
        if (canCook)
        {
            var kitchen = entityManager.GetActiveEntity(Building.KITCHEN);
            yield return movement.Move(kitchen.transform.position);
            yield return new WaitForSeconds(workDuration);
            resourcesManager.ConsumeResources(breadRecipe.GetIngredients());
            var servingTable = entityManager.GetActiveEntity(Building.SERVING_TABLE);
            yield return movement.Move(servingTable.transform.position);
            resourcesManager.AddResource("bread", 1);
            foodOrderManager.ReadyToServeFood();
        }
        yield return null;
    }
    private IEnumerator DoNothing()
    {
        var randomPos = GetRandomPositionInScreen();
        yield return movement.Move(randomPos);
        yield return new WaitForSeconds(1.5f);
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
