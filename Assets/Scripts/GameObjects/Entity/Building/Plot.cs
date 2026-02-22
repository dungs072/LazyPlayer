using System.Collections;
using System.Diagnostics;
using UnityEngine;
using FoodType = ResourceConstant.Food.FoodType;
public enum PlotState
{
    EMPTY,
    GROWING,
    READY
}

public class Plot : BuildableEntity
{
    [SerializeField] private float growthTime = 10f;
    [SerializeField] private SpriteRenderer plotRenderer;
    private PlotState state = PlotState.EMPTY;

    private string currentCropId = string.Empty;
    private int harvestedAmount = 0;
    public bool IsEmpty => state == PlotState.EMPTY;
    public bool IsReady => state == PlotState.READY;
    protected override void Awake()
    {
        base.Awake();
        SwitchState(PlotState.EMPTY);
    }

    public void PlantCrop(string cropId, int harvestedAmount)
    {
        if (state != PlotState.EMPTY)
        {
            UnityEngine.Debug.LogWarning("Plot is not empty!");
            return;
        }
        currentCropId = cropId;
        this.harvestedAmount = harvestedAmount;
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        SwitchState(PlotState.GROWING);
        yield return new WaitForSeconds(growthTime);
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
            plotRenderer.color = Color.gray;
        }
        else if (state == PlotState.GROWING)
        {
            plotRenderer.color = Color.green;
        }
        else if (state == PlotState.READY)
        {
            plotRenderer.color = Color.yellow;
        }
    }
}
