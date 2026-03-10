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
        Plot plot = QueryBus.Query<GetEmptyPlotQuery, Plot>(new GetEmptyPlotQuery()); 
        while (plot != null)
        {
            await movement.Move(cancellationToken, plot.transform.position);
            
            plot.PlantCrop("wheat", 10);
            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);
            
            plot = QueryBus.Query<GetEmptyPlotQuery, Plot>(new GetEmptyPlotQuery()); 
        }
        plot = QueryBus.Query<GetHarvestablePlotQuery, Plot>(new GetHarvestablePlotQuery()); 
        
        while (plot == null)
        {
            await DoNothing(cancellationToken);
            
            plot = QueryBus.Query<GetHarvestablePlotQuery, Plot>(new GetHarvestablePlotQuery()); 
        }
        
        var storage = QueryBus.Query<GetActiveEntityQuery, Entity>(new GetActiveEntityQuery(Building.FARM_STORAGE));
        while (plot != null)
        {
            await movement.Move(cancellationToken, plot.transform.position);
            
            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);
            
            var harvestedCrop = plot.Harvest();
            await movement.Move(cancellationToken, storage.transform.position);
            
            EventBus.Publish(new AddResourceEvent(harvestedCrop.Item1, harvestedCrop.Item2));
            plot = QueryBus.Query<GetHarvestablePlotQuery, Plot>(new GetHarvestablePlotQuery()); 
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
