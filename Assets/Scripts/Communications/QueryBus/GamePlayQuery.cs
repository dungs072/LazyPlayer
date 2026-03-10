using UnityEngine;

public struct GetCenterCameraPositionQuery: IQueryResult<Vector3> { }

public struct GetEntityQuery: IQueryResult<Entity>
{
    public string prefabId;
    public Vector3 position;
}

public struct GetSnapGridPositionQuery: IQueryResult<Vector3>
{
    public Vector3 position;
}

public struct GetActiveEntityQuery: IQueryResult<Entity>
{
    public string prefabId;

    public GetActiveEntityQuery(string prefabId)
    {
        this.prefabId = prefabId;
    }
}

public struct GetEmptyPlotQuery: IQueryResult<Plot>
{
}

public struct GetHarvestablePlotQuery : IQueryResult<Plot>
{
}

public struct IsAvailableFoodQuery: IQueryResult<bool>
{
    public FoodAmount[] foodAmounts;
    
    public IsAvailableFoodQuery(FoodAmount[] foodAmounts)
    {
        this.foodAmounts = foodAmounts;
    }
}


public struct IsAvailableToCreateFoodQuery: IQueryResult<bool>
{
    public IngredientAmount[] ingredientAmounts;

    public IsAvailableToCreateFoodQuery(IngredientAmount[] ingredientAmounts)
    {
        this.ingredientAmounts = ingredientAmounts;
    }
}

public struct GetRecipeDataQuery: IQueryResult<RecipeData>
{
    public string recipeName;
    
    public GetRecipeDataQuery(string recipeName)
    {
        this.recipeName = recipeName;
    }
}

public struct GetOldestFoodOrderQuery: IQueryResult<FoodOrder> { }
