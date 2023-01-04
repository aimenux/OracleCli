namespace App.Services.Oracle;

public sealed class OracleObject
{
    public string OwnerName { get; init; }
    public string ObjectName { get; init; }
    public string ObjectType { get; init; }
    public DateTime CreationDate { get; init; }
}