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

    public static EntityPrefabId GetPrefabId(string prefabName)
    {
        return prefabName switch
        {
            "Entity" => ENTITY,
            "BuildableEntity" => BUILDABLE_ENTITY,
            _ => throw new System.Exception($"Prefab ID not found for prefab name: {prefabName}"),
        };
    }
}
