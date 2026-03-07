using UnityEngine;

public struct GetCenterCameraPositionQuery { }
public struct GetEntityQuery
{
    public string entityName;
    public Vector3 position;
}
public struct GetSnapGridPositionQuery
{
    public Vector3 position;
}

public struct GetActiveEntityQuery
{
    public string entityName;
    
    public GetActiveEntityQuery(string entityName)
    {
        this.entityName = entityName;
    }
}

public struct GetEmptyPlotQuery
{
    public string entityName;
    public GetEmptyPlotQuery(string entityName)
    {
        this.entityName = entityName;
    }
}

public struct GetHarvestablePlotQuery
{
    public string entityName;
    public GetHarvestablePlotQuery(string entityName)
    {
        this.entityName = entityName;
    }
}