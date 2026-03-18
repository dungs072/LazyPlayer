using System;
using System.Collections.Generic;
using UnityEngine;

public struct GetRecipeDataQuery : IQueryResult<RecipeData>
{
    public RecipeId Id;

    public GetRecipeDataQuery(RecipeId id)
    {
        Id = id;
    }
}

public struct GetCropDataQuery : IQueryResult<CropData>
{
    public CropId Id;

    public GetCropDataQuery(CropId id)
    {
        Id = id;
    }
}

public class FoodDictionary : MonoBehaviour
{
    [field: SerializeField] public IngredientData[] Ingredients { get; private set; }
    [field: SerializeField] public RecipeData[] Recipes { get; private set; }
    [field: SerializeField] public CropData[] Crops { get; private set; }

    private readonly Dictionary<RecipeId, RecipeData> recipeDictionary = new();
    private readonly Dictionary<InventoryItemId, IngredientData> ingredientDictionary = new();
    private readonly Dictionary<CropId, CropData> cropDictionary = new();

    public void Initialize1()
    {
        for (int i = 0; i < Recipes.Length; i++)
        {
            recipeDictionary[Recipes[i].GetId()] = Recipes[i];
        }
        for (int i = 0; i < Ingredients.Length; i++)
        {
            ingredientDictionary[Ingredients[i].GetId()] = Ingredients[i];
        }

        for (int i = 0; i < Crops.Length; i++)
        {
            cropDictionary[Crops[i].GetId()] = Crops[i];
        }
        
        QueryBus.Subscribe<GetRecipeDataQuery, RecipeData>(query => GetRecipeData(query.Id));
        QueryBus.Subscribe<GetCropDataQuery, CropData>(query => GetCropData(query.Id));
    }
    public RecipeData GetRecipeData(RecipeId recipeId)
    {
        if (recipeDictionary.TryGetValue(recipeId, out var recipeData))
        {
            return recipeData;
        }
        Debug.LogError($"Recipe '{recipeId}' not found in FoodDictionary.");
        return null;
    }

    public CropData GetCropData(CropId cropId)
    {
        if (cropDictionary.TryGetValue(cropId, out var cropData))
        {
            return cropData;
        }
        Debug.LogError($"Crop '{cropId}' not found in FoodDictionary.");
        return null;
    }
    
    public IngredientData GetIngredientData(InventoryItemId ingredientId)
    {
        if (ingredientDictionary.TryGetValue(ingredientId, out var ingredientData))
        {
            return ingredientData;
        }
        Debug.LogError($"Ingredient '{ingredientId}' not found in FoodDictionary.");
        return null;
    }
}
