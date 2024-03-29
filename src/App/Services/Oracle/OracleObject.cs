namespace App.Services.Oracle;

public sealed class OracleObject : IEquatable<OracleObject>
{
    public string SchemaName { get; init; }
    public string ObjectName { get; init; }
    public string ObjectType { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime ModificationDate { get; init; }

    public bool Equals(OracleObject other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SchemaName == other.SchemaName &&
               ObjectName == other.ObjectName &&
               ObjectType == other.ObjectType &&
               CreationDate.Equals(other.CreationDate) &&
               ModificationDate.Equals(other.ModificationDate);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleObject other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SchemaName, ObjectName, ObjectType, CreationDate, ModificationDate);
    }
}