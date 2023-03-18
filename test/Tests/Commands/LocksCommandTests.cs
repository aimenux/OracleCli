using App.Commands;
using App.Configuration;
using App.Services.Oracle;
using FluentAssertions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Commands;

[Collection(Collections.OracleCollectionName)]
public class LocksCommandTests
{
    private readonly OracleFixture _oracleFixture;
    
    private const string DatabaseName = "oracle-for-tests";

    public LocksCommandTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }

    [Theory]
    [InlineData(null, 0, 5)]
    [InlineData("SYS", 1, 5)]
    [InlineData("SYSTEM", 2, 5)]
    public async Task Should_LocksCommand_Return_Ok(string schemaName, int minTime, int maxItems)
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
        var command = new LocksCommand(consoleService, oracleService, options)
        {
            DatabaseName = DatabaseName,
            SchemaName = schemaName,
            MinBlockingTimeInMinutes = minTime,
            MaxItems = maxItems
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ok);
    }

    [Theory]
    [InlineData(null, -1, 5)]
    [InlineData(null, 3001, 5)]
    [InlineData(null, 1, 0)]
    [InlineData(null, 1, 5001)]
    public async Task Should_LocksCommand_Return_Ko(string schemaName, int minTime, int maxItems)
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
        var command = new LocksCommand(consoleService, oracleService, options)
        {
            DatabaseName = DatabaseName,
            SchemaName = schemaName,
            MinBlockingTimeInMinutes = minTime,
            MaxItems = maxItems
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ko);
    }
}