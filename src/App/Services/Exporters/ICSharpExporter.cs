using App.Services.Oracle;

namespace App.Services.Exporters;

public interface ICSharpExporter
{
    string ExportOracleArguments(ICollection<OracleArgument> oracleArguments, OracleParameters parameters);
}