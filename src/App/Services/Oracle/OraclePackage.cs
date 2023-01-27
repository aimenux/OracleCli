namespace App.Services.Oracle;

public sealed class OraclePackage
{
    public string OwnerName { get; init; }
    public string PackageName { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime ModificationDate { get; init; }
}