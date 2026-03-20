using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Plot : Entity
{
    [SerializeField]
    private SpriteRenderer cropRenderer;
    public bool IsEmpty => currentCrop == null;
    public bool IsReady =>
        currentCrop != null && currentGrowthState == currentCrop.GetCropGrowthStates().Count;

    private CropData currentCrop;
    private int currentGrowthState = -1;

    public void PlantCrop(CropId cropId)
    {
        if (currentGrowthState != -1)
        {
            Debug.LogWarning("Plot is not empty!");
            return;
        }
        GrowCropAsync(cropId).Forget();
    }

    private async UniTask GrowCropAsync(CropId cropId)
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

            await UniTask.Delay(System.TimeSpan.FromSeconds(growthTime));
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
}
