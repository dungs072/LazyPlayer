using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CropGrowthState
{
    public Sprite StateImage;
    public int GrowthTimeSecond;
}

[CreateAssetMenu(fileName = "New Crop", menuName = "Food/CropData")]
public class CropData : ScriptableObject
{
    [SerializeField] private string Id;
    [SerializeField] private string DisplayName;
    [SerializeField] private List<CropGrowthState> CropGrowthStates = new();
    //TODO: Should we redefine another "IngredientAmount" struct?
    [SerializeField] private List<IngredientAmount> HarvestAmounts = new();
    
    public string GetId() => Id;
    public string GetDisplayName() => DisplayName;
    public List<CropGrowthState> GetCropGrowthStates() => CropGrowthStates;
    public List<IngredientAmount> GetHarvestAmounts() => HarvestAmounts;
}