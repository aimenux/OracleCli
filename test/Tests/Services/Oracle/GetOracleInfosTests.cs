using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOracleInfosTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleInfosTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Fact]
    public async Task Should_Get_Infos()
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
            DatabaseName = databaseName
        };

        var service = new OracleService(options, logger);

        // act
        var info = await service.GetOracleInfoAsync(oracleArgs, CancellationToken.None);

        // assert
        info.Should().NotBeNull();
    }
}