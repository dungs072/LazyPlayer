using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer cropRenderer;
    public bool IsEmpty => currentCrop == null;
    public bool IsReady =>
        currentCrop != null && currentGrowthState == currentCrop.GetCropGrowthStates().Count;

    private CropData currentCrop;
    private int currentGrowthState = -1;
    private CancellationTokenSource growCts;

    public void PlantCrop(CropId cropId)
    {
        if (currentGrowthState != -1)
        {
            Debug.LogWarning("Plot is not empty!");
            return;
        }
        growCts?.Cancel();
        growCts = new CancellationTokenSource();
        GrowCropAsync(cropId, growCts.Token).Forget();
    }

    private async UniTask GrowCropAsync(CropId cropId, CancellationToken token)
    {
        currentCrop = QueryBus.Query(new GetCropDataQuery(cropId));

        if (currentCrop == null)
        {
            Debug.LogError("current crop is null!");
            return;
        }

        currentGrowthState = 0;
        while (currentGrowthState < currentCrop.GetCropGrowthStates().Count)
        {
            var currentCropState = currentCrop.GetCropGrowthStates()[currentGrowthState];
            cropRenderer.sprite = currentCropState.StateImage;

            var growthTime = currentCropState.GrowthTimeSecond;

            await UniTask.Delay(System.TimeSpan.FromSeconds(growthTime), cancellationToken: token);
            currentGrowthState += 1;
        }
    }

    public List<IngredientAmount> HarvestCrop()
    {
        if (currentCrop == null)
        {
            Debug.LogError("current crop is null!");
            return new List<IngredientAmount>();
        }

        if (currentGrowthState != currentCrop.GetCropGrowthStates().Count)
        {
            Debug.LogWarning("Plot is not ready for harvest!");
            return new List<IngredientAmount>();
        }

        var current = currentCrop;
        currentCrop = null;
        currentGrowthState = -1;
        cropRenderer.sprite = null;
        return current.GetHarvestAmounts();
    }

    public void ClearCrop()
    {
        growCts?.Cancel();
        currentCrop = null;
        currentGrowthState = -1;
        cropRenderer.sprite = null;
    }
}
