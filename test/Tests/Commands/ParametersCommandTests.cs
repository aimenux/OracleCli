using App.Commands;
using App.Configuration;
using App.Services.Exporters;
using App.Services.Oracle;
using FluentAssertions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Commands;

[Collection(Collections.OracleCollectionName)]
public class ParametersCommandTests
{
    private readonly OracleFixture _oracleFixture;
    
    private const string DatabaseName = "oracle-for-tests";

    public ParametersCommandTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Theory]
    [InlineData(null, null, "###", null)]
    [InlineData(null, null, "RUN", null)]
    [InlineData(null, null, "UPPER", null)]
    [InlineData(null, null, "LOWER", null)]
    [InlineData(null, null, "LENGTH", null)]
    [InlineData(null, null, "GET_LINE", null)]
    [InlineData(null, "RMJVM", "RUN", null)]
    [InlineData("SYS", "DBMS_AW", "RUN", null)]
    [InlineData("SYS", null, "GET_TEXT", null)]
    [InlineData("SYS", null, "GET_LINE", null)]
    [InlineData("SYS", "UTL_TCP", "GET_TEXT", null)]
    [InlineData("SYS", "DBMS_OUTPUT", "GET_LINE", null)]
    [InlineData(null, null, null, "###")]
    [InlineData(null, null, null, "GREATEST")]
    [InlineData(null, null, null, "GET_LINE")]
    [InlineData(null, null, null, "TO_MULTI_BYTE")]
    [InlineData(null, null, null, "TO_SINGLE_BYTE")]
    [InlineData("SYS", null, null, "GREATEST")]
    [InlineData("SYS", null, null, "TO_MULTI_BYTE")]
    [InlineData("SYS", null, null, "TO_SINGLE_BYTE")]
    [InlineData("SYS", "STANDARD", null, "GREATEST")]
    [InlineData("SYS", "STANDARD", null, "TO_MULTI_BYTE")]
    [InlineData("SYS", "STANDARD", null, "TO_SINGLE_BYTE")]
    public async Task Should_ParametersCommand_Return_Ok(string schemaName, string packageName, string procedureName, string functionName)
    {
        // arrange
        var connectionString = _oracleFixture.ConnectionString;
        
        var settings = new SettingsBuilder()
            .WithDatabase(DatabaseName, connectionString)
            .Build();
        
        var options = Options.Create(settings);

        var app = new CommandLineApplication();
        var consoleService = new FakeConsoleService();
        var logger = NullLogger<OracleService>.Instance;
        var oracleService = new OracleService(options, logger);
        var exportService = Substitute.For<ICSharpExportService>();
        var command = new ParametersCommand(consoleService, oracleService, exportService, options)
        {
            DatabaseName = DatabaseName,
            SchemaName = schemaName,
            PackageName = packageName,
            ProcedureName = procedureName,
            FunctionName = functionName
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ok);
    }
    
    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData("SYS", null, null, null)]
    [InlineData("SYS", "STANDARD", null, null)]
    [InlineData("SYS", "STANDARD", "", null)]
    [InlineData("SYS", "STANDARD", null, "")]
    [InlineData("SYS", "STANDARD", "", "")]
    public async Task Should_ParametersCommand_Return_Ko(string schemaName, string packageName, string procedureName, string functionName)
    {
        // arrange
        var connectionString = _oracleFixture.ConnectionString;
        
        var settings = new SettingsBuilder()
            .WithDatabase(DatabaseName, connectionString)
            .Build();
        
        var options = Options.Create(settings);
        
        var app = new CommandLineApplication();
        var consoleService = new FakeConsoleService();
        var logger = NullLogger<OracleService>.Instance;
        var oracleService = new OracleService(options, logger);
        var exportService = Substitute.For<ICSharpExportService>();
        var command = new ParametersCommand(consoleService, oracleService, exportService, options)
        {
            DatabaseName = DatabaseName,
            SchemaName = schemaName,
            PackageName = packageName,
            ProcedureName = procedureName,
            FunctionName = functionName
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ko);
    }
}