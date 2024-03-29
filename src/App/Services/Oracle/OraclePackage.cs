namespace App.Services.Oracle;

public sealed class OraclePackage : IEquatable<OraclePackage>
{
    public string SchemaName { get; init; }
    public string PackageName { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime ModificationDate { get; init; }

    public bool Equals(OraclePackage other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SchemaName == other.SchemaName 
               && PackageName == other.PackageName 
               && CreationDate.Equals(other.CreationDate) 
               && ModificationDate.Equals(other.ModificationDate);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OraclePackage other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SchemaName, PackageName, CreationDate, ModificationDate);
    }
}