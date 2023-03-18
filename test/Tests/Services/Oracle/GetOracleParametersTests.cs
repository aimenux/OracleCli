using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOracleParametersTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleParametersTests(OracleFixture oracleFixture)
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
    public async Task Should_Get_Parameters(string ownerName, string packageName, string procedureName, string functionName)
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
            OwnerName = ownerName,
            DatabaseName = databaseName,
            ProcedureName = procedureName,
            FunctionName = functionName,
            PackageName = packageName
        };

        var service = new OracleService(options, logger);

        // act
        var oracleParameters = await service.GetOracleParametersAsync(oracleArgs, CancellationToken.None);

        // assert
        oracleParameters.Should().NotBeEmpty();
    }
}