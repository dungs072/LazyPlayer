using UnityEngine;

public struct GetCenterCameraPositionQuery: IQueryResult<Vector3> { }

public struct GetEntityQuery: IQueryResult<Entity>
{
    public string entityName;
    public Vector3 position;
}

public struct GetSnapGridPositionQuery: IQueryResult<Vector3>
{
    public Vector3 position;
}

public struct GetActiveEntityQuery: IQueryResult<Entity>
{
    public string entityName;

    public GetActiveEntityQuery(string entityName)
    {
        this.entityName = entityName;
    }
}

public struct GetEmptyPlotQuery: IQueryResult<Plot>
{
    public string entityName;

    public GetEmptyPlotQuery(string entityName)
    {
        this.entityName = entityName;
    }
}

public struct GetHarvestablePlotQuery: IQueryResult<Plot>
{
    public string entityName;

    public GetHarvestablePlotQuery(string entityName)
    {
        this.entityName = entityName;
    }
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
