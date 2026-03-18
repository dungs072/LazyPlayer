using System;

//TODO: restructure file
public readonly struct CropId : IEquatable<CropId>
{
    public readonly string Value;
    
    private CropId(string value)
    {
        Value = value;
    }
    
    public override string ToString() => Value;
    
    public static readonly CropId CROP_WHEAT = new("crop_wheat");
    
    public static CropId ParseId(string id)
    {
        return id switch
        {
            "crop_wheat" => CROP_WHEAT,
            _ => throw new System.Exception($"Food id not found for value {id}")
        };
    }

    public bool Equals(CropId other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        return obj is CropId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (Value != null ? Value.GetHashCode() : 0);
    }
}
