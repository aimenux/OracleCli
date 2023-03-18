using App.Services.Oracle;

namespace App.Services.Exporters;

public interface ICSharpExportService
{
    Task ExportOracleParametersAsync(ICollection<OracleParameter> oracleParameters, OracleArgs args, CancellationToken cancellationToken);
}