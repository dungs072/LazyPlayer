using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Building = EntityConstant.Building;
public class Farmer : BaseWorker
{
    private float workDuration = 2f;
    public Farmer(float workDuration) : base()
    {
        this.workDuration = workDuration;
    }
    public override string JobName()
    {
        return "Farmer";
    }

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var entityManager = GameManager.Instance.GamePlay.EntityManager;
        Plot plot = entityManager.GetEmptyPlot(Building.PLOT);
        while (plot != null)
        {
            await movement.Move(cancellationToken, plot.transform.position);
            
            plot.PlantCrop("wheat", 10);
            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);
            
            plot = entityManager.GetEmptyPlot(Building.PLOT);
        }
        plot = entityManager.GetHarvestablePlot(Building.PLOT);
        while (plot == null)
        {
            await DoNothing(cancellationToken);
            
            plot = entityManager.GetHarvestablePlot(Building.PLOT);
        }
        var storage = entityManager.GetActiveEntity(Building.FARM_STORAGE);
        var resourcesManager = GameManager.Instance.GamePlay.ResourcesManager;
        while (plot != null)
        {
            await movement.Move(cancellationToken, plot.transform.position);
            
            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);
            
            var harvestedCrop = plot.Harvest();
            await movement.Move(cancellationToken, storage.transform.position);
            
            resourcesManager.AddResource(harvestedCrop.Item1, harvestedCrop.Item2);
            plot = entityManager.GetHarvestablePlot(Building.PLOT);
        }
    }
    private async UniTask DoNothing(CancellationToken cancellationToken)
    {
        var randomPos = GetRandomPositionInFarmMap();
        await movement.Move(cancellationToken, randomPos);
        await UniTask.WaitForSeconds(1.5f, cancellationToken: cancellationToken);
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
