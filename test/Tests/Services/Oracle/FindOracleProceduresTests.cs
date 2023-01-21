using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class FindOracleProceduresTests
{
    private readonly OracleFixture _oracleFixture;

    public FindOracleProceduresTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Fact]
    public async Task Should_Find_Procedures()
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
        var procedures = await service.FindOracleProceduresAsync(parameters, CancellationToken.None);

        // assert
        procedures.Should().NotBeEmpty();
        procedures.Should().HaveCount(1);
        procedures.Single().PackageName.Should().Be(parameters.PackageName);
        procedures.Single().ProcedureName.Should().Be(parameters.ProcedureName);
    }
}