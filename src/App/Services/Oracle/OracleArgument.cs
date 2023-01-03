namespace App.Services.Oracle;

public sealed class OracleArgument
{
    public string Name { get; init; }
    public int Position { get; init; }
    public string DataType { get; init; }
    public string Direction { get; init; }
}