using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Tests.Services;

[Collection(Collections.OracleCollectionName)]
public class GetOracleFunctionsTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleFunctionsTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Fact]
    public async Task Should_Get_Functions()
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
        var functions = await service.GetOracleFunctionsAsync(parameters, CancellationToken.None);

        // assert
        functions.Should().NotBeEmpty();
    }
}