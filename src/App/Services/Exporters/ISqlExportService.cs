using App.Services.Oracle;

namespace App.Services.Exporters;

public interface ISqlExportService
{
    Task ExportOracleSourcesAsync(ICollection<OracleSource> oracleSources, OracleArgs args, CancellationToken cancellationToken);
}