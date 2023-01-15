namespace App.Services.Oracle;

public sealed class OracleProcedure
{
    public string OwnerName { get; init; }
    public string PackageName { get; init; }
    public string ProcedureName { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime ModificationDate { get; init; }
}