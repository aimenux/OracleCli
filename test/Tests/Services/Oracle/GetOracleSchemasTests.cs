using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOracleSchemasTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleSchemasTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("SYS")]
    [InlineData("OUTLN")]
    [InlineData("SYSTEM")]
    public async Task Should_Get_Schemas(string filterKeyword)
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
        var schemas = await service.GetOracleSchemasAsync(oracleArgs, CancellationToken.None);

        // assert
        schemas.Should().NotBeEmpty();
    }
}