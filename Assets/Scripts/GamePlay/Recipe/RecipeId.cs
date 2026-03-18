using System;

//TODO: restructure file
public readonly struct RecipeId: IEquatable<RecipeId>
{
    public readonly string Value;
    
    public RecipeId(string value)
    {
        Value = value;
    }
    
    public override string ToString() => Value;
    
    public static readonly RecipeId BREAD = new RecipeId("bread");

    public static RecipeId ParseId(string id)
    {
        return id switch
        {
            "bread" => BREAD,
            _ => throw new Exception($"Bread id not found for value {id}")
        };
    }

    public bool Equals(RecipeId other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        return obj is RecipeId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (Value != null ? Value.GetHashCode() : 0);
    }
}