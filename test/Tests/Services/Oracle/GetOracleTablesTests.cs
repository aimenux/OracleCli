using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOracleTablesTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleTablesTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Theory]
    [InlineData("SYSTEM", "VDK_DATABASE")]
    [InlineData("SYSTEM", "VDK_DATAFILE")]
    [InlineData("SYSTEM", "VDK_INSTANCE")]
    public async Task Should_Get_Table(string schemaName, string tableName)
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
            SchemaName = schemaName,
            TableName = tableName
        };

        var service = new OracleService(options, logger);

        // act
        var table = await service.GetOracleTableAsync(oracleArgs, CancellationToken.None);

        // assert
        table.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("SQL")]
    public async Task Should_Get_Tables(string filterKeyword)
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
        var tables = await service.GetOracleTablesAsync(oracleArgs, CancellationToken.None);

        // assert
        tables.Should().NotBeEmpty();
    }
}