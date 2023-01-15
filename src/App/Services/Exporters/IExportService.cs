using App.Services.Oracle;

namespace App.Services.Exporters;

public interface IExportService
{
    string ExportOracleArguments(ICollection<OracleArgument> oracleArguments, OracleParameters parameters);
}