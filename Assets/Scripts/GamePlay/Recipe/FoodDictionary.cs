using System.Collections.Generic;
using UnityEngine;

public class FoodDictionary : MonoBehaviour
{
    [field: SerializeField] public IngredientData[] Ingredients { get; private set; }
    [field: SerializeField] public RecipeData[] Recipes { get; private set; }


    private readonly Dictionary<string, RecipeData> recipeDictionary = new();
    private readonly Dictionary<string, IngredientData> ingredientDictionary = new();

    void Awake()
    {
        for (int i = 0; i < Recipes.Length; i++)
        {
            recipeDictionary[Recipes[i].GetId()] = Recipes[i];
        }
        for (int i = 0; i < Ingredients.Length; i++)
        {
            ingredientDictionary[Ingredients[i].GetId()] = Ingredients[i];
        }
    }
    public RecipeData GetRecipeData(string name)
    {
        if (recipeDictionary.TryGetValue(name, out var recipeData))
        {
            return recipeData;
        }
        Debug.LogError($"Recipe '{name}' not found in FoodDictionary.");
        return null;
    }
    public IngredientData GetIngredientData(string name)
    {
        if (ingredientDictionary.TryGetValue(name, out var ingredientData))
        {
            return ingredientData;
        }
        Debug.LogError($"Ingredient '{name}' not found in FoodDictionary.");
        return null;
    }
}
