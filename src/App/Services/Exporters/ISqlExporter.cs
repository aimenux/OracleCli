using App.Services.Oracle;

namespace App.Services.Exporters;

public interface ISqlExporter
{
    string ExportOracleSources(ICollection<OracleSource> oracleSources, OracleParameters parameters);
    string ExportOracleSourcesErrors(ICollection<OracleSource> oracleSources, OracleParameters parameters);
}