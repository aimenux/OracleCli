using System.Reflection;
using System.Text;
using System.Text.Json;
using App.Configuration;
using App.Services.Oracle;
using App.Validators;
using Spectre.Console;
using TextCopy;

namespace App.Services.Console;

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

    public void RenderOracleObjects(ICollection<OracleObject> oracleObjects, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var title = oracleObjects.Count > parameters.MaxItems
            ? $"[yellow][bold]Found more than {parameters.MaxItems} object(s)[/][/]"
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
        var count = Math.Min(oracleObjects.Count, parameters.MaxItems);
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

    public void RenderOracleSchemas(ICollection<OracleSchema> oracleSchemas, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var title = oracleSchemas.Count > parameters.MaxItems
            ? $"[yellow][bold]Found more than {parameters.MaxItems} schema(s)[/][/]"
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
        var count = Math.Min(oracleSchemas.Count, parameters.MaxItems);
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

    public void RenderOracleSources(ICollection<OracleSource> oracleSources, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var errors = CountOracleSourcesErrors(parameters.ErrorsFile);
        var lines = oracleSources.Count(x => !string.IsNullOrWhiteSpace(x.Text));
        var table = new Table().BorderColor(Color.White).Border(TableBorder.Square);
        if (errors > 0)
        {
            table = table
                .Title($"[yellow][bold]Found {lines} line(s) and {errors} error(s)[/][/]")
                .AddColumn(new TableColumn("[u]PackageName[/]").Centered())
                .AddColumn(new TableColumn("[u]ProcedureName[/]").Centered())
                .AddColumn(new TableColumn("[u]OutputFile[/]").Centered())
                .AddColumn(new TableColumn("[u]ErrorsFile[/]").Centered())
                .Caption($"[yellow][bold]{databaseName}[/][/]");

            table.AddRow(
                ToMarkup(parameters.PackageName),
                ToMarkup(parameters.ProcedureName),
                ToMarkupLink(parameters.OutputFile),
                ToMarkupLink(parameters.ErrorsFile));
        }
        else
        {
            table = table
                .Title($"[yellow][bold]Found {lines} line(s)[/][/]")
                .AddColumn(new TableColumn("[u]PackageName[/]").Centered())
                .AddColumn(new TableColumn("[u]ProcedureName[/]").Centered())
                .AddColumn(new TableColumn("[u]OutputFile[/]").Centered())
                .Caption($"[yellow][bold]{databaseName}[/][/]");

            table.AddRow(
                ToMarkup(parameters.PackageName),
                ToMarkup(parameters.ProcedureName),
                ToMarkupLink(parameters.OutputFile));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOraclePackages(ICollection<OraclePackage> oraclePackages, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var title = oraclePackages.Count > parameters.MaxItems
            ? $"[yellow][bold]Found more than {parameters.MaxItems} package(s)[/][/]"
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
            .AddColumn(new TableColumn("[u]ProceduresCount[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oraclePackages.Count, parameters.MaxItems);
        foreach (var result in oraclePackages.Take(count))
        {
            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.OwnerName),
                ToMarkup(result.PackageName),
                ToMarkup(result.CreationDate.ToString("g")),
                ToMarkup(result.ModificationDate.ToString("g")),
                ToMarkup($"{result.ProceduresCount}"));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleArguments(ICollection<OracleArgument> oracleArguments, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
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

    public void RenderOracleFunctions(ICollection<OracleFunction> oracleFunctions, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var title = oracleFunctions.Count > parameters.MaxItems
            ? $"[yellow][bold]Found more than {parameters.MaxItems} function(s)[/][/]"
            : $"[yellow][bold]Found {oracleFunctions.Count} function(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]FunctionName[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .AddColumn(new TableColumn("[u]ModificationDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(oracleFunctions.Count, parameters.MaxItems);
        foreach (var result in oracleFunctions.Take(count))
        {
            table.AddRow(
                IndexMarkup(index++),
                ToMarkup(result.OwnerName),
                ToMarkup(result.FunctionName),
                ToMarkup(result.CreationDate.ToString("g")),
                ToMarkup(result.ModificationDate.ToString("g")));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var title = oracleProcedures.Count > parameters.MaxItems
            ? $"[yellow][bold]Found more than {parameters.MaxItems} procedure(s)[/][/]"
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
        var count = Math.Min(oracleProcedures.Count, parameters.MaxItems);
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

    public void RenderFoundOracleProcedures(ICollection<OracleProcedure> oracleProcedures, OracleParameters parameters)
    {
        RenderProblem($"Found {oracleProcedures.Count} procedure(s) matching name '{parameters.ProcedureName}'");
        if (oracleProcedures.Count == 0) return;
        if (!AnsiConsole.Confirm("Do you want to [u]display found procedure(s) on console screen[/] ?"))
        {
            AnsiConsole.WriteLine();
        }
        else
        {
            RenderOracleProcedures(oracleProcedures, parameters);            
        }
    }

    private static int CountOracleSourcesErrors(string errorsFile)
    {
        if (!File.Exists(errorsFile)) return 0;
        var count = File.ReadAllLines(errorsFile)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .Count();
        return count;
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