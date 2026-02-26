using UnityEngine;
using System;
[Serializable]
public class IngredientAmount
{
    public IngredientData ingredient;
    public int amount;
}
[CreateAssetMenu(fileName = "New Recipe", menuName = "Food/RecipeData")]
public class RecipeData : ScriptableObject
{
    [SerializeField] private string Id;          // "tomato_soup"
    [SerializeField] private string DisplayName; // "Tomato Soup"
    [SerializeField] private Sprite Icon;
    [SerializeField] private IngredientAmount[] Ingredients;

    public string GetId() => Id;
    public IngredientAmount[] GetIngredients() => Ingredients;



}
