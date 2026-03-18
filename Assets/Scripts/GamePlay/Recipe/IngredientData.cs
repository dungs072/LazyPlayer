using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Food/IngredientData")]
public class IngredientData : ScriptableObject
{
    [SerializeField] private string Id;          // "tomato"
    [SerializeField] private string DisplayName; // "Tomato"
    [SerializeField] private Sprite Icon;

    public InventoryItemId GetId() => InventoryItemId.ParseId(Id);
    public string GetDisplayName() => DisplayName;
    public Sprite GetIcon() => Icon;
}
