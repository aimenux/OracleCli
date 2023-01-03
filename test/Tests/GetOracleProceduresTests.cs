using App.Configuration;
using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Tests;

[Collection(Collections.OracleCollectionName)]
public class GetOracleProceduresTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleProceduresTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Fact]
    public async Task Should_Get_Procedures()
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
        var procedures = await service.GetOracleProceduresAsync(parameters, CancellationToken.None);

        // assert
        procedures.Should().NotBeEmpty();
    }
}