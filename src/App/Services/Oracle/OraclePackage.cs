namespace App.Services.Oracle;

public sealed class OraclePackage
{
    public string OwnerName { get; init; }
    public string PackageName { get; init; }
    public int ProceduresCount { get; init; }
}