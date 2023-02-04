using System.Text;
using App.Services.Oracle;
using Humanizer;

namespace App.Services.Exporters;

public class CSharpExportService : ICSharpExportService
{
    private readonly ITextExportService _textExportService;

    public CSharpExportService(ITextExportService textExportService)
    {
        _textExportService = textExportService ?? throw new ArgumentNullException(nameof(textExportService));
    }

    public async Task ExportOracleArgumentsAsync(ICollection<OracleArgument> oracleArguments, OracleParameters parameters, CancellationToken cancellationToken)
    {
        var name = GetCSharpName(parameters);
        var type = GetCSharpType(parameters);

        var csharpBuilder = new StringBuilder
        (
            $$"""
            // code generated by oracle-cli at {{DateTime.Now:F}}

            [OracleProperty("{{name}}")]
            public class {{type}}
            {
                {{GetCSharpArguments(oracleArguments)}}                
            }

            """
        );

        csharpBuilder.AppendLine($"{GetCSharpCursorClasses(oracleArguments)}");

        csharpBuilder.AppendLine
        (
            """
            public class OraclePropertyAttribute : Attribute
            {
                public string Name { get; }

                public OraclePropertyAttribute(string name)
                {
                    Name = name;
                }
            }
            """
        );

        var csharpText = csharpBuilder.ToString();
        await _textExportService.ExportToClipboardAsync(csharpText, cancellationToken);
    }
    
    private static string GetCSharpCursorClasses(IEnumerable<OracleArgument> oracleArguments)
    {
        var csharpBuilder = new StringBuilder();
        
        foreach (var oracleArgument in oracleArguments.Where(x => x.IsCursorType))
        {
            var cursorClassType = GetCSharpCursorType(oracleArgument);
            csharpBuilder.AppendLine();
            csharpBuilder.AppendLine
            (
                $$"""
                public class {{cursorClassType}}
                {
                    // cursor properties generation is not supported currently by oracle-cli
                }
                """
            );
        }

        return csharpBuilder.ToString();
    }

    private static string GetCSharpArguments(IEnumerable<OracleArgument> oracleArguments)
    {
        var csharpBuilder = new StringBuilder();
        
        foreach (var oracleArgument in oracleArguments)
        {
            var name = GetCSharpName(oracleArgument);
            var type = GetCSharpType(oracleArgument);
            csharpBuilder.AppendLine();
            csharpBuilder.AppendLine
            (
                $$"""
                    [OracleProperty("{{oracleArgument.Name}}")]
                    public {{type}} {{name}} { get; set; }
                """
            );
        }

        return csharpBuilder.ToString();
    }

    private static string GetCSharpName(OracleArgument oracleArgument)
    {
        var name = string.IsNullOrWhiteSpace(oracleArgument.Name)
            ? $"{oracleArgument.Direction}{oracleArgument.Position}"
            : oracleArgument.Name;
        
        return name.Transform(To.LowerCase).Pascalize();
    }

    private static string GetCSharpType(OracleArgument oracleArgument)
    {
        return oracleArgument.DataType.ToUpper() switch
        {
            "NUMBER" => "long",
            "VARCHAR" => "string",
            "VARCHAR2" => "string",
            "DATE" => "DateTime",
            "REF CURSOR" => GetCSharpCursorType(oracleArgument),
            _ => "NotSupportedType"
        };
    }

    private static string GetCSharpCursorType(OracleArgument oracleArgument)
    {
        return $"IEnumerable<OracleCursor{oracleArgument.Position}>";
    }

    private static string GetCSharpName(OracleParameters parameters)
    {
        var schemaName = parameters.OwnerName.ToUpper();
        var pkgName = parameters.PackageName?.ToUpper();
        var spcName = parameters.ProcedureName?.ToUpper();
        var funName = parameters.FunctionName?.ToUpper();
        var prgName = spcName ?? funName;
        return string.IsNullOrWhiteSpace(pkgName) 
            ? $"{schemaName}.{prgName}" 
            : $"{schemaName}.{pkgName}.{prgName}";
    }
    
    private static string GetCSharpType(OracleParameters parameters)
    {
        return string.IsNullOrWhiteSpace(parameters.FunctionName)
            ? "OracleProcedure"
            : "OracleFunction";
    }
}