using App.Configuration;

namespace App.Services.Oracle;

public sealed class OracleParameters
{
    public string OwnerName { get; init; }
    public string TableName { get; init; }
    public string PackageName { get; init; }
    public string ProcedureName { get; init; }
    public string DatabaseName { get; init; }
    public string FilterKeyword { get; init; }
    public string[] ObjectTypes { get; init; }
    public string OutputDirectory { get; init; }
    public string OutputFile { get; init; }
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;
}

public static class OracleParametersExtensions
{
    public static OracleParameters With(this OracleParameters parameters, string ownerName, string tableName)
    {
        return new OracleParameters
        {
            DatabaseName = parameters.DatabaseName,
            TableName = tableName,
            OwnerName = ownerName,
            PackageName = parameters.PackageName,
            ProcedureName = parameters.ProcedureName,
            MaxItems = parameters.MaxItems,
            FilterKeyword = parameters.FilterKeyword,
            ObjectTypes = parameters.ObjectTypes,
            OutputDirectory = parameters.OutputDirectory,
            OutputFile = parameters.OutputFile
        };
    }
    
    public static OracleParameters With(this OracleParameters parameters, string ownerName, string packageName, string procedureName)
    {
        return new OracleParameters
        {
            DatabaseName = parameters.DatabaseName,
            TableName = parameters.TableName,
            OwnerName = ownerName,
            PackageName = packageName,
            ProcedureName = procedureName,
            MaxItems = parameters.MaxItems,
            FilterKeyword = parameters.FilterKeyword,
            ObjectTypes = parameters.ObjectTypes,
            OutputDirectory = parameters.OutputDirectory,
            OutputFile = parameters.OutputFile
        };
    }
}