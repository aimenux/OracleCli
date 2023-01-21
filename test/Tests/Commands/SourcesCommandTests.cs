using App.Commands;
using App.Configuration;
using App.Services.Console;
using App.Services.Exporters;
using App.Services.Oracle;
using FluentAssertions;
using McMaster.Extensions.CommandLineUtils;
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
    [InlineData("SYS", "RMJVM", "RUN")]
    [InlineData("SYS", "RMJVM", "STRIP")]
    public async Task Should_SourcesCommand_Return_Ok(string ownerName, string packageName, string procedureName)
    {
        // arrange
        var connectionString = _oracleFixture.ConnectionString;
        
        var settings = new SettingsBuilder()
            .WithDatabase(DatabaseName, connectionString)
            .Build();
        
        var options = Options.Create(settings);
        
        var app = new CommandLineApplication();
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = new OracleService(options);
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
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = new OracleService(options);
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