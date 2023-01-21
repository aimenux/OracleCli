using App.Services.Exporters;
using App.Services.Oracle;
using NSubstitute;

namespace Tests.Services.Exporters;

public class CSharpExportServiceTests
{
    [Fact]
    public async Task Should_Export_OracleArguments_To_CSharp()
    {
        // arrange
        var textExportService = Substitute.For<ITextExportService>();
        var oracleParameters = new OracleParameters
        {
            OwnerName = "SYS",
            ProcedureName = "GET_INFO"
        };
        var oracleArguments = new List<OracleArgument>
        {
            new()
            {
                Name = "PI_NAME",
                Direction = "IN",
                DataType = "VARCHAR",
                Position = 1
            }
        };

        var csharpExportService = new CSharpExportService(textExportService);

        // act
        await csharpExportService.ExportOracleArgumentsAsync(oracleArguments, oracleParameters, CancellationToken.None);

        // assert
        await textExportService
            .Received(1)
            .ExportToClipboardAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}