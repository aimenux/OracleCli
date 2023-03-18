using App.Configuration;

namespace App.Services.Oracle;

public sealed class OracleArgs
{
    public string OwnerName { get; init; }
    public string TableName { get; init; }
    public string PackageName { get; init; }
    public string ProcedureName { get; init; }
    public string FunctionName { get; init; }
    public string DatabaseName { get; init; }
    public string FilterKeyword { get; init; }
    public string[] ObjectTypes { get; init; }
    public string OutputDirectory { get; init; }
    public string OutputFile { get; init; }
    public int MinBlockingTimeInMinutes { get; init; }
    public int MaxItems { get; init; } = Settings.DatabaseMaxItems;
}

public static class OracleArgsExtensions
{
    public static OracleArgs WithTable(this OracleArgs args, string ownerName, string tableName)
    {
        return new OracleArgs
        {
            DatabaseName = args.DatabaseName,
            TableName = tableName,
            OwnerName = ownerName,
            PackageName = args.PackageName,
            ProcedureName = args.ProcedureName,
            FunctionName = args.FunctionName,
            MaxItems = args.MaxItems,
            FilterKeyword = args.FilterKeyword,
            ObjectTypes = args.ObjectTypes,
            OutputDirectory = args.OutputDirectory,
            OutputFile = args.OutputFile,
            MinBlockingTimeInMinutes = args.MinBlockingTimeInMinutes
        };
    }
    
    public static OracleArgs WithFunction(this OracleArgs args, string ownerName, string packageName, string functionName)
    {
        return new OracleArgs
        {
            DatabaseName = args.DatabaseName,
            TableName = args.TableName,
            OwnerName = ownerName,
            PackageName = packageName,
            ProcedureName = args.ProcedureName,
            FunctionName = functionName,
            MaxItems = args.MaxItems,
            FilterKeyword = args.FilterKeyword,
            ObjectTypes = args.ObjectTypes,
            OutputDirectory = args.OutputDirectory,
            OutputFile = args.OutputFile,
            MinBlockingTimeInMinutes = args.MinBlockingTimeInMinutes
        };
    }
    
    public static OracleArgs WithProcedure(this OracleArgs args, string ownerName, string packageName, string procedureName)
    {
        return new OracleArgs
        {
            DatabaseName = args.DatabaseName,
            TableName = args.TableName,
            OwnerName = ownerName,
            PackageName = packageName,
            ProcedureName = procedureName,
            FunctionName = args.FunctionName,
            MaxItems = args.MaxItems,
            FilterKeyword = args.FilterKeyword,
            ObjectTypes = args.ObjectTypes,
            OutputDirectory = args.OutputDirectory,
            OutputFile = args.OutputFile,
            MinBlockingTimeInMinutes = args.MinBlockingTimeInMinutes
        };
    }
}