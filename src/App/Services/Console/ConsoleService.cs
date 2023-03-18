using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json;
using App.Configuration;
using App.Extensions;
using App.Services.Oracle;
using App.Validators;
using Humanizer;
using Spectre.Console;
using TextCopy;

namespace App.Services.Console;

[ExcludeFromCodeCoverage]
public class ConsoleService : IConsoleService
{
    public ConsoleService()
    {
        System.Console.OutputEncoding = Encoding.UTF8;
    }

    public void RenderTitle(string text)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new FigletText(text));
        AnsiConsole.WriteLine();
    }

    public void RenderVersion(string version)
    {
        var text = $"{Settings.Cli.FriendlyName} V{version}";
        RenderText(text, Color.White);
    }

    public void RenderProblem(string text) => RenderText(text, Color.Red);

    public void RenderText(string text, Color color)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup($"[bold {color}]{text}[/]"));
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }

    public void RenderSettingsFile(string filepath)
    {
        var name = Path.GetFileName(filepath);
        var json = File.ReadAllText(filepath);
        var formattedJson = GetFormattedJson(json);
        var header = new Rule($"[yellow]({name})[/]");
        header.Centered();
        var footer = new Rule($"[yellow]({filepath})[/]");
        footer.Centered();

        AnsiConsole.WriteLine();
        AnsiConsole.Write(header);
        AnsiConsole.WriteLine(formattedJson);
        AnsiConsole.Write(footer);
        AnsiConsole.WriteLine();
    }
    
    public void RenderUserSecretsFile(string filepath)
    {
        if (!OperatingSystem.IsWindows()) return;
        if (!File.Exists(filepath)) return;
        if (!GetYesOrNoAnswer("display user secrets", false)) return;
        RenderSettingsFile(filepath);
    }

    public void RenderException(Exception exception) => RenderAnyException(exception);

    public static void RenderAnyException<T>(T exception) where T : Exception
    {
        const ExceptionFormats formats = ExceptionFormats.ShortenTypes
                                         | ExceptionFormats.ShortenPaths
                                         | ExceptionFormats.ShortenMethods;

        AnsiConsole.WriteLine();
        AnsiConsole.WriteException(exception, formats);
        AnsiConsole.WriteLine();
    }

    public async Task RenderStatusAsync(Func<Task> action)
    {
        var spinner = RandomSpinner();

        await AnsiConsole.Status()
            .StartAsync("Work is in progress ...", async ctx =>
            {
                ctx.Spinner(spinner);
                await action.Invoke();
            });
    }

    public async Task<T> RenderStatusAsync<T>(Func<Task<T>> func)
    {
        var spinner = RandomSpinner();

        return await AnsiConsole.Status()
            .StartAsync("Work is in progress ...", async ctx =>
            {
                ctx.Spinner(spinner);
                return await func.Invoke();
            });
    }
    
    public bool GetYesOrNoAnswer(string text, bool defaultAnswer)
    {
        if (AnsiConsole.Confirm($"Do you want to [u]{text}[/] ?", defaultAnswer)) return true;
        AnsiConsole.WriteLine();
        return false;
    }

    public void RenderValidationErrors(ValidationErrors validationErrors)
    {
        var count = validationErrors.Count;

        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title($"[red][bold]{count} error(s)[/][/]")
            .AddColumn(new TableColumn("[u]Name[/]").Centered())
            .AddColumn(new TableColumn("[u]Message[/]").Centered())
            .Caption("[grey][bold]Invalid options/arguments[/][/]");

        foreach (var error in validationErrors)
        {
            var failure = error.Failure;
            var name = $"[bold]{error.OptionName()}[/]";
            var reason = $"[tan]{failure.ErrorMessage}[/]";
            table.AddRow(ToMarkup(name), ToMarkup(reason));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public async Task CopyTextToClipboardAsync(string text, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            await ClipboardService.SetTextAsync(text, cancellationToken);            
        }
    }

    public void RenderOracleInfo(OracleInfo oracleInfo, OracleArgs args)
    {
        var description = oracleInfo.Description
            .Replace("Production", "")
            .Trim(' ', '-');
        var databaseName = args.DatabaseName.ToUpper();
        var title = $"[yellow][bold]{description}[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]InstanceName[/]").Centered())
            .AddColumn(new TableColumn("[u]HostName[/]").Centered())
            .AddColumn(new TableColumn("[u]Version[/]").Centered())
            .AddColumn(new TableColumn("[u]LogMode[/]").Centered())
            .AddColumn(new TableColumn("[u]OpenMode[/]").Centered())
            .AddColumn(new TableColumn("[u]ProtectionMode[/]").Centered())
            .AddColumn(new TableColumn("[u]InstanceStatus[/]").Centered())
            .AddColumn(new TableColumn("[u]DatabaseStatus[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .AddColumn(new TableColumn("[u]StartupDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        table.AddRow(
            ToMarkup(oracleInfo.InstanceName),
            ToMarkup(oracleInfo.HostName),
            ToMarkup(oracleInfo.Version),
            ToMarkup(oracleInfo.LogMode),
            ToMarkup(oracleInfo.OpenMode),
            ToMarkup(oracleInfo.ProtectionMode),
            ToMarkup(oracleInfo.InstanceStatus),
            ToMarkup(oracleInfo.DatabaseStatus),
            ToMarkup(oracleInfo.CreationDate.ToString("g")),
            ToMarkup(oracleInfo.StartupDate.ToString("g")));

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleTable(OracleTable oracleTable, OracleArgs args)
    {
        var ownerName = args.OwnerName.ToUpper();
        var tableName = args.TableName.ToUpper();
        var databaseName = args.DatabaseName.ToUpper();
        var title = $"[yellow][bold]Table {ownerName}.{tableName}[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]ColumnName[/]").Centered())
            .AddColumn(new TableColumn("[u]ColumnType[/]").Centered())
            .AddColumn(new TableColumn("[u]IsNullable[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        foreach (var column in oracleTable.TableColumns)
        {
            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(column.Name),
                ToMarkup(column.Type),
                ToMarkup(column.Nullable));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleTables(ICollection<OracleTable> oracleTables, OracleArgs args)
    {
        var databaseName = args.DatabaseName.ToUpper();
        var title = oracleTables.Count > args.MaxItems
            ? $"[yellow][bold]Found more than {args.MaxItems} table(s)[/][/]"
            : $"[yellow][bold]Found {oracleTables.Count} table(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]TableName[/]").Centered())
            .AddColumn(new TableColumn("[u]RowsCount[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oracleTables.Count, args.MaxItems);
        foreach (var result in oracleTables.Take(count))
        {
            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.OwnerName),
                ToMarkup(result.TableName),
                ToMarkup($"{result.RowsCount}"));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleObjects(ICollection<OracleObject> oracleObjects, OracleArgs args)
    {
        var databaseName = args.DatabaseName.ToUpper();
        var title = oracleObjects.Count > args.MaxItems
            ? $"[yellow][bold]Found more than {args.MaxItems} object(s)[/][/]"
            : $"[yellow][bold]Found {oracleObjects.Count} object(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]ObjectName[/]").Centered())
            .AddColumn(new TableColumn("[u]ObjectType[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .AddColumn(new TableColumn("[u]ModificationDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oracleObjects.Count, args.MaxItems);
        foreach (var result in oracleObjects.Take(count))
        {
            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.OwnerName),
                ToMarkup(result.ObjectName),
                ToMarkup(result.ObjectType),
                ToMarkup(result.CreationDate.ToString("g")),
                ToMarkup(result.ModificationDate.ToString("g")));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleSchemas(ICollection<OracleSchema> oracleSchemas, OracleArgs args)
    {
        var databaseName = args.DatabaseName.ToUpper();
        var title = oracleSchemas.Count > args.MaxItems
            ? $"[yellow][bold]Found more than {args.MaxItems} schema(s)[/][/]"
            : $"[yellow][bold]Found {oracleSchemas.Count} schema(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]SchemaName[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oracleSchemas.Count, args.MaxItems);
        foreach (var result in oracleSchemas.Take(count))
        {
            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.SchemaName),
                ToMarkup(result.CreationDate.ToString("g")));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleSources(ICollection<OracleSource> oracleSources, OracleArgs args)
    {
        var isProcedure = !string.IsNullOrWhiteSpace(args.ProcedureName);
        var column = isProcedure ? "ProcedureName" : "FunctionName";
        var @object = isProcedure ? "procedure" : "function"; 
        var name = isProcedure
            ? args.ProcedureName
            : args.FunctionName;
        
        if (oracleSources.Count == 0)
        {
            RenderProblem($"Code source is not found for {@object} '{name}'");
            return;
        }

        var databaseName = args.DatabaseName.ToUpper();
        var lines = oracleSources.Count(x => !string.IsNullOrWhiteSpace(x.Text));
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title($"[yellow][bold]Found {lines} line(s)[/][/]")
            .AddColumn(new TableColumn("[u]PackageName[/]").Centered())
            .AddColumn(new TableColumn($"[u]{column}[/]").Centered())
            .AddColumn(new TableColumn("[u]OutputFile[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");
        
        table.AddRow(
            ToMarkup(args.PackageName),
            ToMarkup(name),
            ToMarkupLink(args.OutputFile));
        
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOraclePackages(ICollection<OraclePackage> oraclePackages, OracleArgs args)
    {
        var databaseName = args.DatabaseName.ToUpper();
        var title = oraclePackages.Count > args.MaxItems
            ? $"[yellow][bold]Found more than {args.MaxItems} package(s)[/][/]"
            : $"[yellow][bold]Found {oraclePackages.Count} package(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]PackageName[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .AddColumn(new TableColumn("[u]ModificationDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oraclePackages.Count, args.MaxItems);
        foreach (var result in oraclePackages.Take(count))
        {
            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.OwnerName),
                ToMarkup(result.PackageName),
                ToMarkup(result.CreationDate.ToString("g")),
                ToMarkup(result.ModificationDate.ToString("g")));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleArguments(ICollection<OracleArgument> oracleArguments, OracleArgs args)
    {
        var databaseName = args.DatabaseName.ToUpper();
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title($"[yellow][bold]Found {oracleArguments.Count} argument(s)[/][/]")
            .AddColumn(new TableColumn("[u]Position[/]").Centered())
            .AddColumn(new TableColumn("[u]Name[/]").Centered())
            .AddColumn(new TableColumn("[u]DataType[/]").Centered())
            .AddColumn(new TableColumn("[u]Direction[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        foreach (var result in oracleArguments)
        {
            table.AddRow(
                ToMarkup($"{result.Position}"),
                ToMarkup(result.Name),
                ToMarkup(result.DataType),
                ToMarkup(result.Direction));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleFunctions(ICollection<OracleFunction> oracleFunctions, OracleArgs args)
    {
        var databaseName = args.DatabaseName.ToUpper();
        var title = oracleFunctions.Count > args.MaxItems
            ? $"[yellow][bold]Found more than {args.MaxItems} function(s)[/][/]"
            : $"[yellow][bold]Found {oracleFunctions.Count} function(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]PackageName[/]").Centered())
            .AddColumn(new TableColumn("[u]FunctionName[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .AddColumn(new TableColumn("[u]ModificationDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oracleFunctions.Count, args.MaxItems);
        foreach (var result in oracleFunctions.Take(count))
        {
            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.OwnerName),
                ToMarkup(result.PackageName),
                ToMarkup(result.FunctionName),
                ToMarkup(result.CreationDate.ToString("g")),
                ToMarkup(result.ModificationDate.ToString("g")));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleArgs args)
    {
        var databaseName = args.DatabaseName.ToUpper();
        var title = oracleProcedures.Count > args.MaxItems
            ? $"[yellow][bold]Found more than {args.MaxItems} procedure(s)[/][/]"
            : $"[yellow][bold]Found {oracleProcedures.Count} procedure(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]PackageName[/]").Centered())
            .AddColumn(new TableColumn("[u]ProcedureName[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .AddColumn(new TableColumn("[u]ModificationDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oracleProcedures.Count, args.MaxItems);
        foreach (var result in oracleProcedures.Take(count))
        {
            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.OwnerName),
                ToMarkup(result.PackageName),
                ToMarkup(result.ProcedureName),
                ToMarkup(result.CreationDate.ToString("g")),
                ToMarkup(result.ModificationDate.ToString("g")));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleLocks(ICollection<OracleLock> oracleLocks, OracleArgs args)
    {
        var databaseName = args.DatabaseName.ToUpper();
        var title = oracleLocks.Count > args.MaxItems
            ? $"[yellow][bold]Found more than {args.MaxItems} lock(s)[/][/]"
            : $"[yellow][bold]Found {oracleLocks.Count} lock(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]SchemaName[/]").Centered())
            .AddColumn(new TableColumn("[u]UserName[/]").Centered())
            .AddColumn(new TableColumn("[u]MachineName[/]").Centered())
            .AddColumn(new TableColumn("[u]ProgramName[/]").Centered())
            .AddColumn(new TableColumn("[u]BlockingSession[/]").Centered())
            .AddColumn(new TableColumn("[u]BlockedSession[/]").Centered())
            .AddColumn(new TableColumn("[u]BlockingStartDate[/]").Centered())
            .AddColumn(new TableColumn("[u]BlockingTime[/]").Centered())
            .AddColumn(new TableColumn("[u]BlockedSqlText[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oracleLocks.Count, args.MaxItems);
        foreach (var result in oracleLocks.Take(count))
        {
            var program = result.ProgramName.Truncate(50);
            var date = result.BlockingStartDate.ToString("g");
            var time = TimeSpan.FromSeconds(result.BlockingTime).Humanize();
            var text = result.BlockedSqlText
                .RemoveLineBreaks()
                .RemoveExtraSpaces()
                .Truncate(100)
                .ToUpper();

            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.SchemaName),
                ToMarkup(result.UserName),
                ToMarkup(result.MachineName),
                ToMarkup(program),
                ToMarkup(result.BlockingSession),
                ToMarkup(result.BlockedSession),
                ToMarkup(date),
                ToMarkup(time),
                ToMarkup($"[grey][bold]{text}[/][/]"));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleSessions(ICollection<OracleSession> oracleSessions, OracleArgs args)
    {
        var databaseName = args.DatabaseName.ToUpper();
        var title = oracleSessions.Count > args.MaxItems
            ? $"[yellow][bold]Found more than {args.MaxItems} active session(s)[/][/]"
            : $"[yellow][bold]Found {oracleSessions.Count} active session(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]SchemaName[/]").Centered())
            .AddColumn(new TableColumn("[u]UserName[/]").Centered())
            .AddColumn(new TableColumn("[u]MachineName[/]").Centered())
            .AddColumn(new TableColumn("[u]ProgramName[/]").Centered())
            .AddColumn(new TableColumn("[u]State[/]").Centered())
            .AddColumn(new TableColumn("[u]LogonDate[/]").Centered())
            .AddColumn(new TableColumn("[u]StartDate[/]").Centered())
            .AddColumn(new TableColumn("[u]SqlText[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oracleSessions.Count, args.MaxItems);
        foreach (var result in oracleSessions.Take(count))
        {
            var module = result.ModuleName.Truncate(50);
            var program = result.ProgramName.Truncate(50);
            if (!string.IsNullOrWhiteSpace(module) && !module.IgnoreEquals(program))
            {
                program = $"{program} ({module})";
            }
            
            var logonDate = result.LogonDate.ToString("g");
            var startDate = result.StartDate.ToString("g");
            var text = result.SqlText
                .RemoveLineBreaks()
                .RemoveExtraSpaces()
                .Truncate(100)
                .ToUpper();

            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.SchemaName),
                ToMarkup(result.UserName),
                ToMarkup(result.MachineName),
                ToMarkup(program),
                ToMarkup(result.State),
                ToMarkup(logonDate),
                ToMarkup(startDate),
                ToMarkup($"[grey][bold]{text}[/][/]"));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static string GetFormattedJson(string json)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        using var document = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(document, options);
    }

    private static Spinner RandomSpinner()
    {
        var values = typeof(Spinner.Known)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.PropertyType == typeof(Spinner))
            .Select(x => (Spinner)x.GetValue(null))
            .ToArray();

        var index = Random.Shared.Next(values.Length);
        var value = (Spinner)values.GetValue(index);
        return value;
    }

    private static Markup ToMarkup(string text)
    {
        try
        {
            return new Markup(text ?? string.Empty);
        }
        catch
        {
            return ErrorMarkup;
        }
    }

    private static readonly Markup ErrorMarkup = new(Emoji.Known.CrossMark);

    private static Markup IndexMarkup(int index) => ToMarkup($"[dim]{index:D4}[/]");
    
    private static Markup ToMarkupLink(string text)
    {
        var link = $"[green][link={text}]{text}[/][/]";
        return ToMarkup(link);
    }
}