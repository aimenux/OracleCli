using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Tests.Services;

[Collection(Collections.OracleCollectionName)]
public class GetOracleSourcesTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleSourcesTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Fact]
    public async Task Should_Get_Sources()
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
            ProcedureName = "SET_USER_ID",
            PackageName = "OWA"
        };

        var service = new OracleService(options);

        // act
        var arguments = await service.GetOracleSourcesAsync(parameters, CancellationToken.None);

        // assert
        arguments.Should().NotBeEmpty();
    }
}