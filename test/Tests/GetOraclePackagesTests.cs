using App.Configuration;
using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Tests;

[Collection(Collections.OracleCollectionName)]
public class GetOraclePackagesTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOraclePackagesTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Fact]
    public async Task Should_Get_Packages()
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
        var packages = await service.GetOraclePackagesAsync(parameters, CancellationToken.None);

        // assert
        packages.Should().NotBeEmpty();
    }
}