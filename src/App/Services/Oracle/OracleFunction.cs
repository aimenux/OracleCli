namespace App.Services.Oracle;

public sealed class OracleFunction
{
    public string OwnerName { get; init; }
    public string FunctionName { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime ModificationDate { get; init; }
}