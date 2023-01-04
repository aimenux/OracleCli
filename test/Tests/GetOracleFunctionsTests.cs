using App.Configuration;
using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Tests;

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
        
        var options = Options.Create(new Settings
        {
            Databases = new List<Database>
            {
                new()
                {
                    DatabaseName = databaseName,
                    ConnectionString = _oracleFixture.ConnectionString
                }
            }
        });

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