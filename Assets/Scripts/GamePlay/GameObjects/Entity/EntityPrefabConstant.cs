using UnityEngine;

public readonly struct EntityPrefabId
{
    public readonly string Value;

    public EntityPrefabId(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;
}

public static class EntityPrefabConstant
{
    public static readonly EntityPrefabId ENTITY = new("Entity");
    public static readonly EntityPrefabId BUILDABLE_ENTITY = new("BuildableEntity");
    public static readonly EntityPrefabId PLOT_ENTITY = new("Plot");
    public static readonly EntityPrefabId FIELD_ENTITY = new("Field");

    public static EntityPrefabId GetPrefabId(string prefabName)
    {
        return prefabName switch
        {
            "Entity" => ENTITY,
            "BuildableEntity" => BUILDABLE_ENTITY,
            "Plot" => PLOT_ENTITY,
            "Field" => FIELD_ENTITY,
            _ => throw new System.Exception($"Prefab ID not found for prefab name: {prefabName}"),
        };
    }
}
