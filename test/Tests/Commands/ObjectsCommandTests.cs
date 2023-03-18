using App.Commands;
using App.Configuration;
using App.Services.Oracle;
using FluentAssertions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Commands;

[Collection(Collections.OracleCollectionName)]
public class ObjectsCommandTests
{
    private readonly OracleFixture _oracleFixture;
    
    private const string DatabaseName = "oracle-for-tests";

    public ObjectsCommandTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }

    [Theory]
    [InlineData(null, null, 5)]
    [InlineData(null, "GET", 5)]
    [InlineData("SYS", "SET", 5)]
    public async Task Should_ObjectsCommand_Return_Ok(string schemaName, string filterKeyword, int maxItems)
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
        var command = new ObjectsCommand(consoleService, oracleService, options)
        {
            DatabaseName = DatabaseName,
            FilterKeyword = filterKeyword,
            SchemaName = schemaName,
            MaxItems = maxItems
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ok);
    }

    [Theory]
    [InlineData(null, "*", 5)]
    [InlineData(null, null, 0)]
    [InlineData(null, null, 5001)]
    public async Task Should_ObjectsCommand_Return_Ko(string schemaName, string filterKeyword, int maxItems)
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
        var command = new ObjectsCommand(consoleService, oracleService, options)
        {
            DatabaseName = DatabaseName,
            FilterKeyword = filterKeyword,
            SchemaName = schemaName,
            MaxItems = maxItems
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ko);
    }
}