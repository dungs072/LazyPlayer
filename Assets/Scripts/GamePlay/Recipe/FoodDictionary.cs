using System.Collections.Generic;
using UnityEngine;

public class FoodDictionary : MonoBehaviour
{
    [field: SerializeField] public IngredientData[] Ingredients { get; private set; }
    [field: SerializeField] public RecipeData[] Recipes { get; private set; }

    private readonly Dictionary<string, RecipeData> recipeDictionary = new();
    private readonly Dictionary<string, IngredientData> ingredientDictionary = new();

    public const string WheatId = "wheat";
    public const string BreadId = "bread";

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
        
        QueryBus.Subscribe<GetRecipeDataQuery, RecipeData>(query => GetRecipeData(query.recipeName));
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
