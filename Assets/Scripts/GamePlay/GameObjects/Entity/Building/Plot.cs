using Cysharp.Threading.Tasks;
using UnityEngine;

public enum PlotState
{
    EMPTY,
    GROWING,
    READY,
}

public class Plot : BuildableEntity
{
    [SerializeField]
    private float growthTime = 10f;

    private PlotState state = PlotState.EMPTY;

    private string currentCropId = string.Empty;
    private int harvestedAmount = 0;
    public bool IsEmpty => state == PlotState.EMPTY;
    public bool IsReady => state == PlotState.READY;

    private void Awake()
    {
        SwitchState(PlotState.EMPTY);
    }

    public void PlantCrop(string cropId, int harvestedAmount)
    {
        if (state != PlotState.EMPTY)
        {
            Debug.LogWarning("Plot is not empty!");
            return;
        }
        currentCropId = cropId;
        this.harvestedAmount = harvestedAmount;
        GrowAsync().Forget();
    }

    private async UniTask GrowAsync()
    {
        SwitchState(PlotState.GROWING);
        await UniTask.Delay(System.TimeSpan.FromSeconds(growthTime));
        SwitchState(PlotState.READY);
    }

    public (string, int) Harvest()
    {
        if (state != PlotState.READY)
        {
            UnityEngine.Debug.LogWarning("Plot is not ready for harvest!");
            return (string.Empty, 0);
        }

        string harvestedCropId = currentCropId;
        int harvestedAmount = this.harvestedAmount;
        currentCropId = string.Empty;
        SwitchState(PlotState.EMPTY);
        return (harvestedCropId, harvestedAmount);
    }

    private void SwitchState(PlotState newState)
    {
        state = newState;
        if (state == PlotState.EMPTY)
        {
            spriteRenderer.color = Color.gray;
        }
        else if (state == PlotState.GROWING)
        {
            spriteRenderer.color = Color.green;
        }
        else if (state == PlotState.READY)
        {
            spriteRenderer.color = Color.yellow;
        }
    }
}
