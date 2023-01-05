using System.Reflection;
using System.Text;
using System.Text.Json;
using App.Services.Oracle;
using App.Validators;
using Spectre.Console;

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
        AnsiConsole.Write(new FigletText(text).LeftAligned());
        AnsiConsole.WriteLine();
    }
    
    public void RenderText(string text, string color)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup($"[bold {color}]{text}[/]"));
        AnsiConsole.WriteLine();
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

    public void RenderOracleObjects(IEnumerable<OracleObject> oracleObjects, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var results = oracleObjects.ToList();
        var title = results.Count > parameters.MaxItems
            ? $"[yellow][bold]Found more than {parameters.MaxItems} object(s)[/][/]"
            : $"[yellow][bold]Found {results.Count} object(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]ObjectName[/]").Centered())
            .AddColumn(new TableColumn("[u]ObjectType[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(results.Count, parameters.MaxItems);
        foreach (var result in results.Take(count))
        {
            table.AddRow(IndexMarkup(index++), ToMarkup(result.OwnerName), ToMarkup(result.ObjectName), ToMarkup(result.ObjectType), ToMarkup(result.CreationDate.ToString("g")));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOraclePackages(IEnumerable<OraclePackage> oraclePackages, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var results = oraclePackages.ToList();
        var title = results.Count > parameters.MaxItems
            ? $"[yellow][bold]Found more than {parameters.MaxItems} package(s)[/][/]"
            : $"[yellow][bold]Found {results.Count} package(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]PackageName[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .AddColumn(new TableColumn("[u]ProceduresCount[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");
        
        var index = 1;
        var count = Math.Min(results.Count, parameters.MaxItems);
        foreach (var result in results.Take(count))
        {
            table.AddRow(IndexMarkup(index++), ToMarkup(result.OwnerName), ToMarkup(result.PackageName), ToMarkup(result.CreationDate.ToString("g")), ToMarkup($"{result.ProceduresCount}"));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleArguments(IEnumerable<OracleArgument> oracleArguments, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var results = oracleArguments.ToList();
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title($"[yellow][bold]Found {results.Count} argument(s)[/][/]")
            .AddColumn(new TableColumn("[u]Position[/]").Centered())
            .AddColumn(new TableColumn("[u]Name[/]").Centered())
            .AddColumn(new TableColumn("[u]DataType[/]").Centered())
            .AddColumn(new TableColumn("[u]Direction[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");
        
        foreach (var result in results)
        {
            table.AddRow(ToMarkup($"{result.Position}"), ToMarkup(result.Name), ToMarkup(result.DataType), ToMarkup(result.Direction));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleFunctions(IEnumerable<OracleFunction> oracleFunctions, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var results = oracleFunctions.ToList();
        var title = results.Count > parameters.MaxItems
            ? $"[yellow][bold]Found more than {parameters.MaxItems} function(s)[/][/]"
            : $"[yellow][bold]Found {results.Count} function(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]FunctionName[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(results.Count, parameters.MaxItems);
        foreach (var result in results.Take(count))
        {
            table.AddRow(IndexMarkup(index++), ToMarkup(result.OwnerName), ToMarkup(result.FunctionName), ToMarkup(result.CreationDate.ToString("g")));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void RenderOracleProcedures(IEnumerable<OracleProcedure> oracleProcedures, OracleParameters parameters)
    {
        var databaseName = parameters.DatabaseName.ToUpper();
        var results = oracleProcedures.ToList();
        var title = results.Count > parameters.MaxItems
            ? $"[yellow][bold]Found more than {parameters.MaxItems} procedure(s)[/][/]"
            : $"[yellow][bold]Found {results.Count} procedure(s)[/][/]";
        var table = new Table()
            .BorderColor(Color.White)
            .Border(TableBorder.Square)
            .Title(title)
            .AddColumn(new TableColumn("[u]#[/]").Centered())
            .AddColumn(new TableColumn("[u]OwnerName[/]").Centered())
            .AddColumn(new TableColumn("[u]PackageName[/]").Centered())
            .AddColumn(new TableColumn("[u]ProcedureName[/]").Centered())
            .AddColumn(new TableColumn("[u]CreationDate[/]").Centered())
            .Caption($"[yellow][bold]{databaseName}[/][/]");

        var index = 1;
        var count = Math.Min(results.Count, parameters.MaxItems);
        foreach (var result in results.Take(count))
        {
            table.AddRow(IndexMarkup(index++), ToMarkup(result.OwnerName), ToMarkup(result.PackageName), ToMarkup(result.ProcedureName), ToMarkup(result.CreationDate.ToString("g")));
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
}