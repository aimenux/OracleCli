namespace App.Services.Oracle;

public sealed class OracleTable : IEquatable<OracleTable>
{
    public string OwnerName { get; init; }
    public string TableName { get; init; }
    public int RowsCount { get; init; }
    public ICollection<OracleColumn> TableColumns { get; init; }

    public bool Equals(OracleTable other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return OwnerName == other.OwnerName 
               && TableName == other.TableName 
               && RowsCount == other.RowsCount 
               && Equals(TableColumns, other.TableColumns);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is OracleTable other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(OwnerName, TableName, RowsCount, TableColumns);
    }
}