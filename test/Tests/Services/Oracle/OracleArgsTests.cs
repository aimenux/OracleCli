using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleArgsTests
{
    [Theory]
    [InlineData("schema1", "package1", "procedure1")]
    [InlineData("schema2", "package2", "procedure2")]
    public void Should_Create_Oracle_Args_WithProcedure(string schemaName, string packageName, string procedureName)
    {
        // arrange
        var oracleArgs1 = new OracleArgs
        {
            DatabaseName = "db",
            OutputDirectory = "c:/files",
            OutputFile = "output.txt",
            SchemaName = "schema",
            PackageName = "package",
            ProcedureName = "procedure",
            FilterKeyword = "*",
            MaxItems = 10
        };

        // act
        var oracleArgs2 = oracleArgs1.WithProcedure(schemaName, packageName, procedureName);

        // assert
        oracleArgs2.DatabaseName.Should().Be(oracleArgs1.DatabaseName);
        oracleArgs2.OutputDirectory.Should().Be(oracleArgs1.OutputDirectory);
        oracleArgs2.OutputFile.Should().Be(oracleArgs1.OutputFile);
        oracleArgs2.FilterKeyword.Should().Be(oracleArgs1.FilterKeyword);
        oracleArgs2.MaxItems.Should().Be(oracleArgs1.MaxItems);
        oracleArgs2.SchemaName.Should().Be(schemaName);
        oracleArgs2.TableName.Should().BeNull();
        oracleArgs2.PackageName.Should().Be(packageName);
        oracleArgs2.ProcedureName.Should().Be(procedureName);
        oracleArgs2.FunctionName.Should().BeNull();
    }
    
    [Theory]
    [InlineData("schema1", "package1", "function1")]
    [InlineData("schema2", "package2", "function2")]
    public void Should_Create_Oracle_Args_WithFunction(string schemaName, string packageName, string functionName)
    {
        // arrange
        var oracleArgs1 = new OracleArgs
        {
            DatabaseName = "db",
            OutputDirectory = "c:/files",
            OutputFile = "output.txt",
            SchemaName = "schema",
            PackageName = "package",
            FunctionName = "function",
            FilterKeyword = "*",
            MaxItems = 10
        };

        // act
        var oracleArgs2 = oracleArgs1.WithFunction(schemaName, packageName, functionName);

        // assert
        oracleArgs2.DatabaseName.Should().Be(oracleArgs1.DatabaseName);
        oracleArgs2.OutputDirectory.Should().Be(oracleArgs1.OutputDirectory);
        oracleArgs2.OutputFile.Should().Be(oracleArgs1.OutputFile);
        oracleArgs2.FilterKeyword.Should().Be(oracleArgs1.FilterKeyword);
        oracleArgs2.MaxItems.Should().Be(oracleArgs1.MaxItems);
        oracleArgs2.SchemaName.Should().Be(schemaName);
        oracleArgs2.TableName.Should().BeNull();
        oracleArgs2.PackageName.Should().Be(packageName);
        oracleArgs2.ProcedureName.Should().BeNull();
        oracleArgs2.FunctionName.Should().Be(functionName);
    }
    
    [Theory]
    [InlineData("schema1", "table1")]
    [InlineData("schema2", "table2")]
    public void Should_Create_Oracle_Args_WithTable(string schemaName, string tableName)
    {
        // arrange
        var oracleArgs1 = new OracleArgs
        {
            DatabaseName = "db",
            TableName = "table",
            OutputDirectory = "c:/files",
            OutputFile = "output.txt",
            SchemaName = "schema",
            FilterKeyword = "*",
            MaxItems = 10
        };

        // act
        var oracleArgs2 = oracleArgs1.WithTable(schemaName, tableName);

        // assert
        oracleArgs2.DatabaseName.Should().Be(oracleArgs1.DatabaseName);
        oracleArgs2.OutputDirectory.Should().Be(oracleArgs1.OutputDirectory);
        oracleArgs2.OutputFile.Should().Be(oracleArgs1.OutputFile);
        oracleArgs2.FilterKeyword.Should().Be(oracleArgs1.FilterKeyword);
        oracleArgs2.MaxItems.Should().Be(oracleArgs1.MaxItems);
        oracleArgs2.SchemaName.Should().Be(schemaName);
        oracleArgs2.TableName.Should().Be(tableName);
        oracleArgs2.PackageName.Should().BeNull();
        oracleArgs2.ProcedureName.Should().BeNull();
        oracleArgs2.FunctionName.Should().BeNull();
    }
}