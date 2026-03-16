using UnityEngine;

//ToDo move them to the file that subcribes the query result
public struct GetCenterCameraPositionQuery : IQueryResult<Vector3> { }

public struct GetEntityPrefabQuery : IQueryResult<Entity>
{
    public string prefabId;
}

public struct GetSnapGridPositionQuery : IQueryResult<Vector3>
{
    public Vector3 position;
}

public struct IsOverlappingGridQuery : IQueryResult<bool>
{
    public Vector3 position;
    public Vector2 size;
}

public struct GetEntityIdAtPositionQuery : IQueryResult<int>
{
    public Vector3 position;
}

public struct GetActiveEntityQuery : IQueryResult<Entity>
{
    public string prefabId;
}

public struct GetEmptyPlotQuery : IQueryResult<Plot> { }

public struct GetHarvestablePlotQuery : IQueryResult<Plot> { }

public struct IsAvailableFoodQuery : IQueryResult<bool>
{
    public FoodAmount[] foodAmounts;

    public IsAvailableFoodQuery(FoodAmount[] foodAmounts)
    {
        this.foodAmounts = foodAmounts;
    }
}

public struct IsAvailableToCreateFoodQuery : IQueryResult<bool>
{
    public IngredientAmount[] ingredientAmounts;

    public IsAvailableToCreateFoodQuery(IngredientAmount[] ingredientAmounts)
    {
        this.ingredientAmounts = ingredientAmounts;
    }
}

public struct GetRecipeDataQuery : IQueryResult<RecipeData>
{
    public string recipeName;

    public GetRecipeDataQuery(string recipeName)
    {
        this.recipeName = recipeName;
    }
}

public struct GetOldestFoodOrderQuery : IQueryResult<FoodOrder> { }
