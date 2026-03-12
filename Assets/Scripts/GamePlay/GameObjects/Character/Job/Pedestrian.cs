using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Pedestrian : BaseWorker
{
    private float workDuration = 2f;
    public override string JobName()
    {
        return "Pedestrian";
    }

    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var startPos = new Vector3(65, -8, 0);
        var endPos = new Vector3(65, 8, 0);
        var randomVar = 0.4f;
        while (randomVar < 0.5f)
        {
            // Move from start to end
            await movementComponent.Move(cancellationToken, endPos);
               await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);
            if (cancellationToken.IsCancellationRequested) return; 

            // Random walk after reaching end
            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                float randomX = Random.value > 0.5f ? startPos.x : startPos.x + 10f; // 2 verticals: 65 and 75
                var randomPos = GetRandomVerticalPosition(randomX);
                await movementComponent.Move(cancellationToken, randomPos);
                       
                await UniTask.WaitForSeconds(Random.Range(0.5f, 2f), cancellationToken: cancellationToken);
            }

            // Move back to start
            await movementComponent.Move(cancellationToken, startPos);
               
            await UniTask.WaitForSeconds(workDuration, cancellationToken: cancellationToken);
            if (cancellationToken.IsCancellationRequested) return; 

            // Random walk after reaching start
            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                float randomX = Random.value > 0.5f ? startPos.x : startPos.x + 10f; // 2 verticals: 65 and 75
                var randomPos = GetRandomVerticalPosition(randomX);
                await movementComponent.Move(cancellationToken, randomPos);
                       
                await UniTask.WaitForSeconds(Random.Range(0.5f, 2f), cancellationToken: cancellationToken);
            }
            randomVar = Random.Range(0f, 1f);
        }
        var diner = new Diner(); 
        character.SetJob(diner);
        diner.DoJob();

    }

    private Vector3 GetRandomVerticalPosition(float x)
    {
        // Use the same x as startPos, randomize y between top and bottom
        float minY = -8f;
        float maxY = 8f;
        float y = Random.Range(minY, maxY);
        return new Vector3(x, y, 0);
    }
}
