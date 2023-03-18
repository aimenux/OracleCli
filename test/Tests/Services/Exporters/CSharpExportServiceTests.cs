using App.Services.Exporters;
using App.Services.Oracle;
using NSubstitute;

namespace Tests.Services.Exporters;

public class CSharpExportServiceTests
{
    [Fact]
    public async Task Should_Export_Procedure_OracleArguments_To_CSharp()
    {
        // arrange
        var textExportService = Substitute.For<ITextExportService>();
        var oracleArgs = new OracleArgs
        {
            OwnerName = "USER",
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
            },
            new()
            {
                Name = "PI_DATE",
                Direction = "IN",
                DataType = "DATE",
                Position = 2
            },
            new()
            {
                Name = "PO_RESULTS",
                Direction = "OUT",
                DataType = "REF CURSOR",
                Position = 3
            }
        };

        var csharpExportService = new CSharpExportService(textExportService);

        // act
        await csharpExportService.ExportOracleArgumentsAsync(oracleArguments, oracleArgs, CancellationToken.None);

        // assert
        await textExportService
            .Received(1)
            .ExportToClipboardAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Export_Function_OracleArguments_To_CSharp()
    {
        // arrange
        var textExportService = Substitute.For<ITextExportService>();
        var oracleArgs = new OracleArgs
        {
            OwnerName = "USER",
            FunctionName = "GET_INFO"
        };
        var oracleArguments = new List<OracleArgument>
        {
            new()
            {
                Name = "",
                Direction = "OUT",
                DataType = "NUMBER",
                Position = 0
            },
            new()
            {
                Name = "PI_NAME",
                Direction = "IN",
                DataType = "VARCHAR",
                Position = 1
            },
            new()
            {
                Name = "PI_DATE",
                Direction = "IN",
                DataType = "DATE",
                Position = 2
            }
        };

        var csharpExportService = new CSharpExportService(textExportService);

        // act
        await csharpExportService.ExportOracleArgumentsAsync(oracleArguments, oracleArgs, CancellationToken.None);

        // assert
        await textExportService
            .Received(1)
            .ExportToClipboardAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}