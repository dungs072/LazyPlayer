using System;

//TODO: restructure file
public readonly struct InventoryItemId : IEquatable<InventoryItemId>
{
    public readonly string Value;

    public InventoryItemId(string value)
    {
        Value = value;
    }
    
    public override string ToString() => Value;
    
    public static readonly InventoryItemId WHEAT = new InventoryItemId("wheat");
    public static readonly InventoryItemId BREAD = new InventoryItemId("bread");
    public static readonly InventoryItemId MONEY = new InventoryItemId("money");

    public static InventoryItemId ParseId(string id)
    {
        return id switch
        {
            "wheat" => WHEAT,
            "bread" => BREAD,
            _ => throw new Exception($"Inventory item id not found for value {id}")
        };
    }

    public static bool operator ==(InventoryItemId left, InventoryItemId right)
    {
        return left.Value == right.Value;
    }
    
    public static bool operator !=(InventoryItemId left, InventoryItemId right)
    {
        return left.Value != right.Value;
    }
    
    public bool Equals(InventoryItemId other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        return obj is InventoryItemId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (Value != null ? Value.GetHashCode() : 0);
    }
}