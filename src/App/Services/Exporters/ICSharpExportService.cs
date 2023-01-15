using App.Services.Oracle;

namespace App.Services.Exporters;

public interface ICSharpExportService
{
    Task ExportOracleArgumentsAsync(ICollection<OracleArgument> oracleArguments, OracleParameters parameters, CancellationToken cancellationToken);
}