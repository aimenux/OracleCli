using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOracleSessionsTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleSessionsTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("SYS")]
    public async Task Should_Get_Sessions(string ownerName)
    {
        // arrange
        const string databaseName = "oracle-for-tests";
        var connectionString = _oracleFixture.ConnectionString;
        
        var settings = new SettingsBuilder()
            .WithDatabase(databaseName, connectionString)
            .Build();
        
        var options = Options.Create(settings);
        var logger = NullLogger<OracleService>.Instance;

        var parameters = new OracleParameters
        {
            DatabaseName = databaseName,
            OwnerName = ownerName,
            MaxItems = 5
        };

        var service = new OracleService(options, logger);

        // act
        var sessions = await service.GetOracleSessionsAsync(parameters, CancellationToken.None);

        // assert
        sessions.Should().NotBeNull().And.BeEmpty();
    }
}