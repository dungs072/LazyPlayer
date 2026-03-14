using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Building = EntityConstant.Building;

public class Farmer : BaseWorker
{
    private float workDuration = 2f;

    public Farmer(float workDuration)
        : base()
    {
        this.workDuration = workDuration;
    }

    public override string JobName()
    {
        return "Farmer";
    }

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        Plot plot = QueryBus.Query(new GetEmptyPlotQuery());
        while (plot != null)
        {
            await character.MovementComponent.Move(cancellationToken, plot.transform.position);

            plot.PlantCrop(FoodDictionary.WheatId, 10);
            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);

            plot = QueryBus.Query(new GetEmptyPlotQuery());
        }
        plot = QueryBus.Query(new GetHarvestablePlotQuery());

        while (plot == null)
        {
            await DoNothing(cancellationToken);

            plot = QueryBus.Query(new GetHarvestablePlotQuery());
        }

        var storage = QueryBus.Query(new GetActiveEntityQuery(Building.FARM_STORAGE));
        while (plot != null)
        {
            await character.MovementComponent.Move(cancellationToken, plot.transform.position);

            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);

            var harvestedCrop = plot.Harvest();
            await character.MovementComponent.Move(cancellationToken, storage.transform.position);

            EventBus.Publish(new AddResourceEvent(harvestedCrop.Item1, harvestedCrop.Item2));
            plot = QueryBus.Query(new GetHarvestablePlotQuery());
        }

        character.EnqueueJob(this);
    }

    private async UniTask DoNothing(CancellationToken cancellationToken)
    {
        var randomPos = GetRandomPositionInFarmMap();
        await character.MovementComponent.Move(cancellationToken, randomPos);
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
