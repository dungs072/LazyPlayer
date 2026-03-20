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
        var entityManager = character.entityManager;
        var plot = GetEmptyPlot();
        while (plot != null)
        {
            await character.MovementComponent.Move(cancellationToken, plot.transform.position);

            plot.PlantCrop(CropId.CROP_WHEAT);
            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);

            plot = GetEmptyPlot();
        }
        plot = GetHarvestablePlot();

        while (plot == null)
        {
            await DoNothing(cancellationToken);

            plot = GetHarvestablePlot();
        }

        var storage = entityManager.FindActiveEntity(EntityId.FARM_STORAGE);
        while (plot != null)
        {
            await character.MovementComponent.Move(cancellationToken, plot.transform.position);

            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);

            var harvestedCrop = plot.HarvestCrop();
            await character.MovementComponent.Move(cancellationToken, storage.transform.position);

            foreach (var ingredientData in harvestedCrop)
            {
                EventBus.Publish(
                    new AddResourceEvent(ingredientData.ingredient.GetId(), ingredientData.amount)
                );
            }
            plot = GetHarvestablePlot();
        }

        character.EnqueueJob(this);
    }

    private Plot GetEmptyPlot()
    {
        var entityManager = character.entityManager;
        var fields = entityManager.FindActiveEntites<Field>(
            EntityId.FIELD,
            (field) =>
            {
                return field.BuildingState == BuildingState.READY;
            }
        );
        foreach (var field in fields)
        {
            var plot = field.FindEmptyPlot();
            if (plot != null)
            {
                return plot;
            }
        }

        return null;
    }

    private Plot GetHarvestablePlot()
    {
        var entityManager = character.entityManager;
        var fields = entityManager.FindActiveEntites<Field>(
            EntityId.FIELD,
            (field) =>
            {
                return field.BuildingState == BuildingState.READY;
            }
        );
        foreach (var field in fields)
        {
            var plot = field.FindHarvestablePlot();
            if (plot != null)
            {
                return plot;
            }
        }
        return null;
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
