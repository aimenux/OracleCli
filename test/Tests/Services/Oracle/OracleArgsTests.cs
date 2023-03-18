using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleArgsTests
{
    [Theory]
    [InlineData("owner1", "package1", "procedure1")]
    [InlineData("owner2", "package2", "procedure2")]
    public void Should_Create_Oracle_Args_WithProcedure(string ownerName, string packageName, string procedureName)
    {
        // arrange
        var oracleArgs1 = new OracleArgs
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
        var oracleArgs2 = oracleArgs1.WithProcedure(ownerName, packageName, procedureName);

        // assert
        oracleArgs2.DatabaseName.Should().Be(oracleArgs1.DatabaseName);
        oracleArgs2.OutputDirectory.Should().Be(oracleArgs1.OutputDirectory);
        oracleArgs2.OutputFile.Should().Be(oracleArgs1.OutputFile);
        oracleArgs2.FilterKeyword.Should().Be(oracleArgs1.FilterKeyword);
        oracleArgs2.MaxItems.Should().Be(oracleArgs1.MaxItems);
        oracleArgs2.OwnerName.Should().Be(ownerName);
        oracleArgs2.TableName.Should().BeNull();
        oracleArgs2.PackageName.Should().Be(packageName);
        oracleArgs2.ProcedureName.Should().Be(procedureName);
        oracleArgs2.FunctionName.Should().BeNull();
    }
    
    [Theory]
    [InlineData("owner1", "package1", "function1")]
    [InlineData("owner2", "package2", "function2")]
    public void Should_Create_Oracle_Args_WithFunction(string ownerName, string packageName, string functionName)
    {
        // arrange
        var oracleArgs1 = new OracleArgs
        {
            DatabaseName = "db",
            OutputDirectory = "c:/files",
            OutputFile = "output.txt",
            OwnerName = "owner",
            PackageName = "package",
            FunctionName = "function",
            FilterKeyword = "*",
            MaxItems = 10
        };

        // act
        var oracleArgs2 = oracleArgs1.WithFunction(ownerName, packageName, functionName);

        // assert
        oracleArgs2.DatabaseName.Should().Be(oracleArgs1.DatabaseName);
        oracleArgs2.OutputDirectory.Should().Be(oracleArgs1.OutputDirectory);
        oracleArgs2.OutputFile.Should().Be(oracleArgs1.OutputFile);
        oracleArgs2.FilterKeyword.Should().Be(oracleArgs1.FilterKeyword);
        oracleArgs2.MaxItems.Should().Be(oracleArgs1.MaxItems);
        oracleArgs2.OwnerName.Should().Be(ownerName);
        oracleArgs2.TableName.Should().BeNull();
        oracleArgs2.PackageName.Should().Be(packageName);
        oracleArgs2.ProcedureName.Should().BeNull();
        oracleArgs2.FunctionName.Should().Be(functionName);
    }
    
    [Theory]
    [InlineData("owner1", "table1")]
    [InlineData("owner2", "table2")]
    public void Should_Create_Oracle_Args_WithTable(string ownerName, string tableName)
    {
        // arrange
        var oracleArgs1 = new OracleArgs
        {
            DatabaseName = "db",
            TableName = "table",
            OutputDirectory = "c:/files",
            OutputFile = "output.txt",
            OwnerName = "owner",
            FilterKeyword = "*",
            MaxItems = 10
        };

        // act
        var oracleArgs2 = oracleArgs1.WithTable(ownerName, tableName);

        // assert
        oracleArgs2.DatabaseName.Should().Be(oracleArgs1.DatabaseName);
        oracleArgs2.OutputDirectory.Should().Be(oracleArgs1.OutputDirectory);
        oracleArgs2.OutputFile.Should().Be(oracleArgs1.OutputFile);
        oracleArgs2.FilterKeyword.Should().Be(oracleArgs1.FilterKeyword);
        oracleArgs2.MaxItems.Should().Be(oracleArgs1.MaxItems);
        oracleArgs2.OwnerName.Should().Be(ownerName);
        oracleArgs2.TableName.Should().Be(tableName);
        oracleArgs2.PackageName.Should().BeNull();
        oracleArgs2.ProcedureName.Should().BeNull();
        oracleArgs2.FunctionName.Should().BeNull();
    }
}