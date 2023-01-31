namespace App.Services.Oracle;

public sealed class OracleFunction : IEquatable<OracleFunction>
{
    public string OwnerName { get; init; }
    public string PackageName { get; init; }
    public string FunctionName { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime ModificationDate { get; init; }

    public bool Equals(OracleFunction other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return OwnerName == other.OwnerName 
               && PackageName == other.PackageName 
               && FunctionName == other.FunctionName 
               && CreationDate.Equals(other.CreationDate) 
               && ModificationDate.Equals(other.ModificationDate);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleFunction other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(OwnerName, PackageName, FunctionName, CreationDate, ModificationDate);
    }
}