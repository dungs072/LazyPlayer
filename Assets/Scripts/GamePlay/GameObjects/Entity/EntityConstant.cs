using System.Collections.Generic;

//Todo: create auto generated code for this file
public readonly struct EntityId
{
    public readonly string Value;

    private EntityId(string value)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString();

    public static readonly EntityId PLOT = new("Plot");
    public static readonly EntityId FARM_STORAGE = new("FarmStorage");
    public static readonly EntityId KITCHEN = new("Kitchen");
    public static readonly EntityId SERVING_TABLE = new("ServingTable");
    public static readonly EntityId DINING_TABLE = new("DiningTable");
    public static readonly EntityId ORDER_TABLE = new("OrderTable");

    public static EntityId ParseId(string value)
    {
        return value switch
        {
            "Plot" => PLOT,
            "FarmStorage" => FARM_STORAGE,
            "Kitchen" => KITCHEN,
            "ServingTable" => SERVING_TABLE,
            "DiningTable" => DINING_TABLE,
            "OrderTable" => ORDER_TABLE,
            _ => throw new System.Exception($"Entity ID not found for value: {value}"),
        };
    }

    public static bool Compare(EntityId id1, EntityId id2)
    {
        return id1.Value == id2.Value;
    }
}

public static class EntityConstant
{
    public static class Building
    {
        public const string NOTHING = "Nothing";
        public const string PLOT = "Plot";
        public const string FARM_STORAGE = "FarmStorage";
        public const string KITCHEN = "Kitchen";
        public const string SERVING_TABLE = "ServingTable";
        public const string DINING_TABLE = "DiningTable";
        public const string ORDER_TABLE = "OrderTable";
    }
}
