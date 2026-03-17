using System.Collections.Generic;
using UnityEngine;

public struct GetRecipeDataQuery : IQueryResult<RecipeData>
{
    public string recipeName;

    public GetRecipeDataQuery(string recipeName)
    {
        this.recipeName = recipeName;
    }
}

public struct GetCropDataQuery : IQueryResult<CropData>
{
    public string cropId;

    public GetCropDataQuery(string cropId)
    {
        this.cropId = cropId;
    }
}

public class FoodDictionary : MonoBehaviour
{
    [field: SerializeField] public IngredientData[] Ingredients { get; private set; }
    [field: SerializeField] public RecipeData[] Recipes { get; private set; }
    [field: SerializeField] public CropData[] Crops { get; private set; }

    private readonly Dictionary<string, RecipeData> recipeDictionary = new();
    private readonly Dictionary<string, IngredientData> ingredientDictionary = new();
    private readonly Dictionary<string, CropData> cropDictionary = new();

    public const string WheatId = "wheat";
    public const string BreadId = "bread";
    public const string CropWheatId = "crop_wheat";   

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
        
        QueryBus.Subscribe<GetRecipeDataQuery, RecipeData>(query => GetRecipeData(query.recipeName));
        QueryBus.Subscribe<GetCropDataQuery, CropData>(query => GetCropData(query.cropId));
    }
    public RecipeData GetRecipeData(string recipeId)
    {
        if (recipeDictionary.TryGetValue(recipeId, out var recipeData))
        {
            return recipeData;
        }
        Debug.LogError($"Recipe '{recipeId}' not found in FoodDictionary.");
        return null;
    }

    public CropData GetCropData(string cropId)
    {
        if (cropDictionary.TryGetValue(cropId, out var cropData))
        {
            return cropData;
        }
        Debug.LogError($"Crop '{cropId}' not found in FoodDictionary.");
        return null;
    }
    
    public IngredientData GetIngredientData(string ingredientId)
    {
        if (ingredientDictionary.TryGetValue(ingredientId, out var ingredientData))
        {
            return ingredientData;
        }
        Debug.LogError($"Ingredient '{ingredientId}' not found in FoodDictionary.");
        return null;
    }
}
