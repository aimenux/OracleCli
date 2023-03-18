using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOracleObjectsTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleObjectsTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("SQL")]
    [InlineData("USR")]
    [InlineData("ORA")]
    public async Task Should_Get_Objects(string filterKeyword)
    {
        // arrange
        const string databaseName = "oracle-for-tests";
        var connectionString = _oracleFixture.ConnectionString;
        
        var settings = new SettingsBuilder()
            .WithDatabase(databaseName, connectionString)
            .Build();
        
        var options = Options.Create(settings);
        var logger = NullLogger<OracleService>.Instance;

        var oracleArgs = new OracleArgs
        {
            DatabaseName = databaseName,
            FilterKeyword = filterKeyword,
            MaxItems = 5
        };

        var service = new OracleService(options, logger);

        // act
        var objects = await service.GetOracleObjectsAsync(oracleArgs, CancellationToken.None);

        // assert
        objects.Should().NotBeEmpty();
    }
    
    [Theory]
    [InlineData("VIEW")]
    [InlineData("TABLE")]
    [InlineData("INDEX")]
    [InlineData("PACKAGE")]
    [InlineData("CLUSTER")]
    [InlineData("SEQUENCE")]
    [InlineData("FUNCTION")]
    [InlineData("PROCEDURE")]
    public async Task Should_Get_Objects_Of_Type(string objectType)
    {
        // arrange
        const string databaseName = "oracle-for-tests";
        var connectionString = _oracleFixture.ConnectionString;
        
        var settings = new SettingsBuilder()
            .WithDatabase(databaseName, connectionString)
            .Build();
        
        var options = Options.Create(settings);
        var logger = NullLogger<OracleService>.Instance;

        var oracleArgs = new OracleArgs
        {
            DatabaseName = databaseName,
            ObjectTypes = new[] { objectType },
            MaxItems = 5
        };

        var service = new OracleService(options, logger);

        // act
        var objects = await service.GetOracleObjectsAsync(oracleArgs, CancellationToken.None);

        // assert
        objects.Should().NotBeEmpty();
    }
}