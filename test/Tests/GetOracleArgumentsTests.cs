using App.Configuration;
using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Tests;

[Collection(Collections.OracleCollectionName)]
public class GetOracleArgumentsTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleArgumentsTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Fact]
    public async Task Should_Get_Arguments()
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
            ProcedureName = "SET_USER_ID",
            PackageName = "OWA"
        };

        var service = new OracleService(options);

        // act
        var arguments = await service.GetOracleArgumentsAsync(parameters, CancellationToken.None);

        // assert
        arguments.Should().NotBeEmpty();
    }
}