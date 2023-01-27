using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOraclePackagesTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOraclePackagesTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("DBMS_SQL")]
    [InlineData("DBMS_JOB")]
    [InlineData("DBMS_LOCK")]
    public async Task Should_Get_Packages(string filterKeyword)
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
            DatabaseName = databaseName,
            FilterKeyword = filterKeyword,
            MaxItems = 5
        };

        var service = new OracleService(options, logger);

        // act
        var packages = await service.GetOraclePackagesAsync(parameters, CancellationToken.None);

        // assert
        packages.Should().NotBeEmpty();
    }
}