namespace App.Services.Oracle;

public sealed class OracleSchema : IEquatable<OracleSchema>
{
    public string SchemaName { get; init; }
    
    public DateTime CreationDate { get; init; }

    public bool Equals(OracleSchema other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SchemaName == other.SchemaName && CreationDate.Equals(other.CreationDate);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleSchema other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SchemaName, CreationDate);
    }
}