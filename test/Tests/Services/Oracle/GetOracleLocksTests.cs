using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOracleLocksTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleLocksTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Theory]
    [InlineData(null, 10)]
    [InlineData("SYS", 20)]
    [InlineData("SYSTEM", 30)]
    public async Task Should_Get_Locks(string ownerName, int minTime)
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
            OwnerName = ownerName,
            MinBlockingTimeInMinutes = minTime,
            MaxItems = 5
        };

        var service = new OracleService(options, logger);

        // act
        var locks = await service.GetOracleLocksAsync(oracleArgs, CancellationToken.None);

        // assert
        locks.Should().NotBeNull().And.BeEmpty();
    }
}