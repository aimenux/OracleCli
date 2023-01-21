using System.Text;
using System.Text.RegularExpressions;
using App.Services.Oracle;
using static System.Environment;

namespace App.Services.Exporters;

public class SqlExportService : ISqlExportService
{
    private readonly ITextExportService _textExportService;

    public SqlExportService(ITextExportService textExportService)
    {
        _textExportService = textExportService ?? throw new ArgumentNullException(nameof(textExportService));
    }
    
    public async Task ExportOracleSourcesAsync(ICollection<OracleSource> oracleSources, OracleParameters parameters, CancellationToken cancellationToken)
    {
        var nonEmptyOracleSources = oracleSources
            .Where(x => !string.IsNullOrWhiteSpace(x.Text))
            .ToList();

        if (!nonEmptyOracleSources.Any()) return;
        
        var sqlBuilder = new StringBuilder
        (
            $$"""
            -- code generated by oracle-cli at {{DateTime.Now:F}}

            """
        );
        
        var errors = GetCustomOracleErrors(nonEmptyOracleSources);
        if (errors.Any())
        {
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine(@"/*");
            foreach (var error in errors)
            {
                sqlBuilder.AppendLine($"* {error}");
            }
            sqlBuilder.AppendLine(@"*/");
            sqlBuilder.AppendLine(NewLine);
            await _textExportService.ExportToClipboardAsync(string.Join(NewLine, errors), cancellationToken);
        }

        foreach (var oracleSource in nonEmptyOracleSources)
        {
            sqlBuilder.Append(oracleSource.Text);
        }

        await _textExportService.ExportToFileAsync(sqlBuilder.ToString(), parameters.OutputFile, cancellationToken);
    }

    private static ICollection<string> GetCustomOracleErrors(ICollection<OracleSource> oracleSources)
    {
        var timeout = TimeSpan.FromSeconds(5);
        const RegexOptions options = RegexOptions.Compiled;
        var sqlText = string.Join("/n", oracleSources.Select(x => x.Text));
        var errors = Regex.Matches(sqlText, @"(=>)(\s)('\w+')", options, timeout)
            .Select(CleanMatch)
            .Distinct()
            .ToList();
        return errors;
    }

    private static string CleanMatch(Match match)
    {
        return match.Value
            .Replace("=", "")
            .Replace(">", "")
            .Replace("'", "")
            .Trim();
    }
}