using App.Services.Oracle;

namespace App.Services.Exporters;

public interface ICSharpExportService
{
    Task ExportOracleArgumentsAsync(ICollection<OracleArgument> oracleArguments, OracleArgs args, CancellationToken cancellationToken);
}