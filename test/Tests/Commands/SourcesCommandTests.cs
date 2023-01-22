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
public class SourcesCommandTests
{
    private readonly OracleFixture _oracleFixture;
    
    private const string DatabaseName = "oracle-for-tests";

    public SourcesCommandTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }

    [Theory]
    [InlineData(null, null, "RUN")]
    [InlineData(null, null, "UPPER")]
    [InlineData(null, null, "LOWER")]
    [InlineData(null, null, "LENGTH")]
    [InlineData(null, "RMJVM", "RUN")]
    [InlineData("SYS", "DBMS_AW", "RUN")]
    [InlineData("SYS", null, "GET_TEXT")]
    [InlineData("SYS", null, "GET_LINE")]
    [InlineData("SYS", "UTL_TCP", "GET_TEXT")]
    [InlineData("SYS", "DBMS_OUTPUT", "GET_LINE")]
    public async Task Should_SourcesCommand_Return_Ok(string ownerName, string packageName, string procedureName)
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
        var exportService = Substitute.For<ISqlExportService>();
        var command = new SourcesCommand(consoleService, oracleService, exportService, options)
        {
            DatabaseName = DatabaseName,
            OwnerName = ownerName,
            PackageName = packageName,
            ProcedureName = procedureName
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ok);
    }
    
    [Theory]
    [InlineData("SYS", "RMJVM", null)]
    [InlineData("SYS", "RMJVM", "")]
    public async Task Should_SourcesCommand_Return_Ko(string ownerName, string packageName, string procedureName)
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
        var exportService = Substitute.For<ISqlExportService>();
        var command = new SourcesCommand(consoleService, oracleService, exportService, options)
        {
            DatabaseName = DatabaseName,
            OwnerName = ownerName,
            PackageName = packageName,
            ProcedureName = procedureName
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ko);
    }
}