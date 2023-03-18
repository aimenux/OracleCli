namespace App.Services.Oracle;

public sealed class OracleProcedure : IEquatable<OracleProcedure>
{
    public string SchemaName { get; init; }
    public string PackageName { get; init; }
    public string ProcedureName { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime ModificationDate { get; init; }

    public bool Equals(OracleProcedure other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SchemaName == other.SchemaName 
               && PackageName == other.PackageName 
               && ProcedureName == other.ProcedureName 
               && CreationDate.Equals(other.CreationDate) 
               && ModificationDate.Equals(other.ModificationDate);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleProcedure other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SchemaName, PackageName, ProcedureName, CreationDate, ModificationDate);
    }
}