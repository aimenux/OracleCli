using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleParametersTests
{
    [Theory]
    [InlineData("owner1", "package1", "procedure1")]
    [InlineData("owner2", "package2", "procedure2")]
    public void Should_Create_Oracle_Parameters(string ownerName, string packageName, string procedureName)
    {
        // arrange
        var oracleParameters1 = new OracleParameters
        {
            DatabaseName = "db",
            OutputDirectory = "c:/files",
            OutputFile = "output.txt",
            OwnerName = "owner",
            PackageName = "package",
            ProcedureName = "procedure",
            FilterKeyword = "*",
            MaxItems = 10
        };

        // act
        var oracleParameters2 = oracleParameters1.With(ownerName, packageName, procedureName);

        // assert
        oracleParameters2.DatabaseName.Should().Be(oracleParameters1.DatabaseName);
        oracleParameters2.OutputDirectory.Should().Be(oracleParameters1.OutputDirectory);
        oracleParameters2.OutputFile.Should().Be(oracleParameters1.OutputFile);
        oracleParameters2.FilterKeyword.Should().Be(oracleParameters1.FilterKeyword);
        oracleParameters2.MaxItems.Should().Be(oracleParameters1.MaxItems);
        oracleParameters2.OwnerName.Should().Be(ownerName);
        oracleParameters2.PackageName.Should().Be(packageName);
        oracleParameters2.ProcedureName.Should().Be(procedureName);
    }
}