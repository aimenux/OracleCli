using App.Extensions;

namespace App.Services.Oracle;

public sealed class OracleParameter : IEquatable<OracleParameter>
{
    public string Name { get; init; }
    public int Position { get; init; }
    public string DataType { get; init; }
    public string Direction { get; init; }
    public bool IsCursorType => DataType.IgnoreEquals("REF CURSOR");

    public bool Equals(OracleParameter other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name 
               && Position == other.Position 
               && DataType == other.DataType 
               && Direction == other.Direction;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleParameter other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Position, DataType, Direction);
    }
}