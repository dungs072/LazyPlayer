using System.Collections;
using UnityEngine;
using static EntityConstant;

public class Pedestrian : BaseWorker
{
    private float workDuration = 2f;
    public override string JobName()
    {
        return "Pedestrian";
    }

    public override IEnumerator DoJobAsync()
    {
        var startPos = new Vector3(65, -8, 0);
        var endPos = new Vector3(65, 8, 0);
        var randomVar = 0.4f;
        while (randomVar < 0.5f)
        {
            // Move from start to end
            yield return movement.Move(endPos);
            yield return new WaitForSeconds(workDuration);

            // Random walk after reaching end
            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                float randomX = Random.value > 0.5f ? startPos.x : startPos.x + 10f; // 2 verticals: 65 and 75
                var randomPos = GetRandomVerticalPosition(randomX);
                yield return movement.Move(randomPos);
                yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            }

            // Move back to start
            yield return movement.Move(startPos);
            yield return new WaitForSeconds(workDuration);

            // Random walk after reaching start
            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                float randomX = Random.value > 0.5f ? startPos.x : startPos.x + 10f; // 2 verticals: 65 and 75
                var randomPos = GetRandomVerticalPosition(randomX);
                yield return movement.Move(randomPos);
                yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            }
            randomVar = Random.Range(0f, 1f);
        }
        var diner = new Diner();
        switchableJob.SetJob(diner);
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
