using App.Services.Exporters;
using App.Services.Oracle;
using NSubstitute;

namespace Tests.Services.Exporters;

public class SqlExportServiceTests
{
    [Fact]
    public async Task Should_Export_OracleSources_To_Sql()
    {
        // arrange
        var textExportService = Substitute.For<ITextExportService>();
        var oracleParameters = new OracleParameters
        {
            OwnerName = "SYS",
            ProcedureName = "GET_INFO"
        };
        var oracleSources = new List<OracleSource>
        {
            new()
            {
                Line = 1,
                Text = "Procedure GET_INFO;"
            }
        };

        var sqlExportService = new SqlExportService(textExportService);

        // act
        await sqlExportService.ExportOracleSourcesAsync(oracleSources, oracleParameters, CancellationToken.None);

        // assert
        await textExportService
            .Received(1)
            .ExportToFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Not_Export_OracleSources_To_Sql()
    {
        // arrange
        var textExportService = Substitute.For<ITextExportService>();
        var oracleParameters = new OracleParameters();
        var oracleSources = new List<OracleSource>();

        var sqlExportService = new SqlExportService(textExportService);

        // act
        await sqlExportService.ExportOracleSourcesAsync(oracleSources, oracleParameters, CancellationToken.None);

        // assert
        await textExportService
            .Received(0)
            .ExportToFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}