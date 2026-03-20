using System.Collections.Generic;
using UnityEngine;

// Field will contain multiple plots

public class Field : BuildableEntity
{
    [SerializeField]
    private List<Plot> plots = new();

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
