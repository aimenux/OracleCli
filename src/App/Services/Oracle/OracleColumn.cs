namespace App.Services.Oracle;

public sealed class OracleColumn : IEquatable<OracleColumn>
{
    public string Name { get; init; }
    public string Type { get; init; }
    public int Position { get; init; }
    public string Nullable { get; init; }

    public bool Equals(OracleColumn other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name 
               && Type == other.Type 
               && Position == other.Position 
               && Nullable == other.Nullable;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleColumn other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Type, Position, Nullable);
    }
}