using System.Collections;
using UnityEngine;
using static EntityConstant;
using FoodType = ResourceConstant.Food.FoodType;
using Building = EntityConstant.Building;
public class Farmer : BaseWorker
{
    private float workDuration = 2f;
    public Farmer(float workDuration) : base()
    {
        this.workDuration = workDuration;
    }

    public override IEnumerator DoJobAsync()
    {
        var entityManager = GameManager.Instance.EntityManager;
        Plot plot = entityManager.GetEmptyPlot(Building.PLOT);
        while (plot != null)
        {
            yield return movement.Move(plot.transform.position);
            plot.PlantCrop("wheat", 10);
            yield return new WaitForSeconds(workDuration);
            plot = entityManager.GetEmptyPlot(Building.PLOT);
        }
        plot = entityManager.GetHarvestablePlot(Building.PLOT);
        while (plot == null)
        {
            yield return DoNothing();
            plot = entityManager.GetHarvestablePlot(Building.PLOT);
        }
        var storage = entityManager.GetActiveEntity(Building.FARM_STORAGE);
        var resourcesManager = GameManager.Instance.ResourcesManager;
        while (plot != null)
        {
            yield return movement.Move(plot.transform.position);
            yield return new WaitForSeconds(workDuration);
            var harvestedCrop = plot.Harvest();
            yield return movement.Move(storage.transform.position);
            resourcesManager.AddResource(harvestedCrop.Item1, harvestedCrop.Item2);
            plot = entityManager.GetHarvestablePlot(Building.PLOT);
        }
    }
    private IEnumerator DoNothing()
    {
        var randomPos = GetRandomPositionInFarmMap();
        yield return movement.Move(randomPos);
        yield return new WaitForSeconds(1.5f);
    }
    public override void FinishCurrentStep()
    {
        currentStepIndex++;
    }

    private Vector3 GetRandomPositionInFarmMap()
    {
        float squareSize = 10f;
        float halfSize = squareSize / 2f;
        float x = Random.Range(-halfSize, halfSize);
        float y = Random.Range(-halfSize, halfSize);
        float z = 0f;
        return new Vector3(x, y, z);
    }
}
