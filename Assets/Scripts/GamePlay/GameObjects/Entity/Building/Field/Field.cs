using System.Collections.Generic;
using UnityEngine;

// Field will contain multiple plots

public class Field : BuildableEntity
{
    [SerializeField]
    private List<Plot> plots = new();

    void OnDisable()
    {
        foreach (var plot in plots)
        {
            plot.ClearCrop();
        }
    }

    public override void ShowVisualForBuilding()
    {
        base.ShowVisualForBuilding();
        foreach (var plot in plots)
        {
            plot.gameObject.SetActive(true);
        }
    }

    public override void HideVisualForBuilding()
    {
        base.HideVisualForBuilding();
        foreach (var plot in plots)
        {
            plot.gameObject.SetActive(false);
        }
    }

    public Plot FindEmptyPlot()
    {
        foreach (var plot in plots)
        {
            if (plot.IsEmpty)
            {
                return plot;
            }
        }
        return null;
    }

    public Plot FindHarvestablePlot()
    {
        foreach (var plot in plots)
        {
            if (plot.IsReady)
            {
                return plot;
            }
        }
        return null;
    }
}
