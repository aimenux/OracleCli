using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOracleSourcesTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleSourcesTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Theory]
    [InlineData("SYS", "OWA", "GET_LINE", null)]
    [InlineData("SYS", "OWA", "SET_USER_ID", null)]
    [InlineData("SYS", "OWA", "SET_PASSWORD", null)]
    [InlineData("SYS", "STANDARD", null, "GREATEST")]
    [InlineData("SYS", "STANDARD", null, "TO_MULTI_BYTE")]
    [InlineData("SYS", "STANDARD", null, "TO_SINGLE_BYTE")]
    public async Task Should_Get_Sources(string ownerName, string packageName, string procedureName, string functionName)
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
            OwnerName = ownerName,
            DatabaseName = databaseName,
            ProcedureName = procedureName,
            FunctionName = functionName,
            PackageName = packageName
        };

        var service = new OracleService(options, logger);

        // act
        var arguments = await service.GetOracleSourcesAsync(parameters, CancellationToken.None);

        // assert
        arguments.Should().NotBeEmpty();
    }
}