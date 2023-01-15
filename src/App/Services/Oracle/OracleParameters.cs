using App.Configuration;

namespace App.Services.Oracle;

public sealed class OracleParameters
{
    public string OwnerName { get; init; }
    public string PackageName { get; init; }
    public string ProcedureName { get; init; }
    public string DatabaseName { get; init; }
    public string FilterKeyword { get; init; }
    public string OutputDirectory { get; init; }
    public string OutputFile { get; init; }
    public string ErrorsFile { get; init; }
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;
}

public static class OracleParametersExtensions
{
    public static OracleParameters With(this OracleParameters parameters, string ownerName, string packageName, string procedureName)
    {
        return new OracleParameters
        {
            DatabaseName = parameters.DatabaseName,
            OwnerName = ownerName,
            PackageName = packageName,
            ProcedureName = procedureName,
            MaxItems = parameters.MaxItems,
            FilterKeyword = parameters.FilterKeyword,
            OutputDirectory = parameters.OutputDirectory,
            OutputFile = parameters.OutputFile,
            ErrorsFile = parameters.ErrorsFile
        };
    }
}