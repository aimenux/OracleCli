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

public static class OracleParametersExtensions
{
    public static OracleParameters With(this OracleParameters parameters, string packageName, string procedureName)
    {
        return new OracleParameters
        {
            DatabaseName = parameters.DatabaseName,
            OwnerName = parameters.OwnerName,
            PackageName = packageName,
            ProcedureName = procedureName,
            MaxItems = parameters.MaxItems,
            FilterKeyword = parameters.FilterKeyword
        };
    }
}