using App.Services.Oracle;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Services.Oracle;

[Collection(Collections.OracleCollectionName)]
public class GetOracleProceduresTests
{
    private readonly OracleFixture _oracleFixture;

    public GetOracleProceduresTests(OracleFixture oracleFixture)
    {
        _oracleFixture = oracleFixture;
    }
    
    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData(null, null, null, "DROP_EXP")]
    [InlineData(null, null, null, "GRANT_EXP")]
    [InlineData(null, null, null, "CREATE_EXP")]
    [InlineData(null, null, "SET_USER_ID", null)]
    [InlineData(null, "OWA", "SET_USER_ID", null)]
    [InlineData("SYS", "DBMS_FILE_GROUP_EXP", "DROP_EXP", null)]
    [InlineData("SYS", "DBMS_FILE_GROUP_EXP", "GRANT_EXP", null)]
    [InlineData("SYS", "DBMS_FILE_GROUP_EXP", "CREATE_EXP", null)]
    public async Task Should_Get_Procedures(string ownerName, string packageName, string procedureName, string filterKeyword)
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
            OwnerName = ownerName,
            PackageName = packageName,
            ProcedureName = procedureName,
            FilterKeyword = filterKeyword,
            MaxItems = 5
        };

        var service = new OracleService(options, logger);

        // act
        var procedures = await service.GetOracleProceduresAsync(parameters, CancellationToken.None);

        // assert
        procedures.Should().NotBeEmpty();
    }
}