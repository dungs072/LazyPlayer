using UnityEngine;

public struct MouseClickEvent
{
    public Vector2 mousePosition;
}

public struct AddResourceEvent
{
    public string resourceName;
    public int amount;

    public AddResourceEvent(string resourceName, int amount)
    {
        this.resourceName = resourceName;
        this.amount = amount;
    }
}

public struct ConsumeResourceEvent
{
    public IngredientAmount[] ingredientAmounts;

    public ConsumeResourceEvent(IngredientAmount[] ingredientAmounts)
    {
        this.ingredientAmounts = ingredientAmounts;
    }
}

public struct ConsumeFoodEvent
{
    public FoodAmount[] foodAmounts;

    public ConsumeFoodEvent(FoodAmount[] foodAmounts)
    {
        this.foodAmounts = foodAmounts;
    }
}

public struct AddFoodOrderEvent
{
    public FoodOrder foodOrder;

    public AddFoodOrderEvent(FoodOrder foodOrder)
    {
        this.foodOrder = foodOrder;
    }
}

public struct ReadyToServeFoodEvent { }

public struct RemoveFoodOrderEvent { }

public struct AddTableOrderEvent
{
    public TableOrder tableOrder;

    public AddTableOrderEvent(TableOrder tableOrder)
    {
        this.tableOrder = tableOrder;
    }
}
