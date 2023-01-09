using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Tests.Services;

[Collection(Collections.OracleCollectionName)]
public class GetOracleObjectsTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleObjectsTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Fact]
    public async Task Should_Get_Objects()
    {
        // arrange
        const string databaseName = "oracle-for-tests";
        var connectionString = _oracleFixture.ConnectionString;
        
        var settings = new SettingsBuilder()
            .WithDatabase(databaseName, connectionString)
            .Build();
        
        var options = Options.Create(settings);

        var parameters = new OracleParameters
        {
            DatabaseName = databaseName,
            MaxItems = 5
        };

        var service = new OracleService(options);

        // act
        var objects = await service.GetOracleObjectsAsync(parameters, CancellationToken.None);

        // assert
        objects.Should().NotBeEmpty();
    }
}