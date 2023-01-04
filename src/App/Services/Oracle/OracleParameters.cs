namespace App.Services.Oracle;

public sealed class OracleParameters
{
    public int MaxItems { get; init; }
    public string OwnerName { get; init; }
    public string PackageName { get; init; }
    public string ProcedureName { get; init; }
    public string DatabaseName { get; init; }
    public string FilterKeyword { get; init; }
}