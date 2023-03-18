using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleTableTests
{
    [Theory]
    [InlineData("Schema1", "Table1", 10)]
    [InlineData("Schema2", "Table2", 20)]
    public void Should_Oracle_Tables_Be_Equals(string schemaName, string tableName, int rowsCount)
    {
        // arrange
        var obj1 = new OracleTable
        {
            SchemaName = schemaName,
            TableName = tableName,
            RowsCount = rowsCount
        };
        var obj2 = new OracleTable
        {
            SchemaName = schemaName,
            TableName = tableName,
            RowsCount = rowsCount
        };

        // act
        var equals1 = obj1.Equals(obj1);
        var equals2 = obj1.Equals(obj2);
        var equals3 = obj1.Equals((object)obj2);

        // assert
        equals1.Should().BeTrue();
        equals2.Should().BeTrue();
        equals3.Should().BeTrue();
    }
    
    [Fact]
    public void Should_Oracle_Tables_Not_Be_Equals()
    {
        // arrange
        var obj1 = new OracleTable
        {
            SchemaName = "schema",
            TableName = "table",
            RowsCount = 1
        };
        
        var obj2 = new OracleTable
        {
            SchemaName = "schema",
            TableName = "table",
            RowsCount = 2
        };
        
        // act
        var equals1 = obj1.Equals(obj2);
        var equals2 = obj1.Equals((object)obj2);
        var equals3 = obj1.Equals(null);
        var equals4 = obj1.Equals((object)null);

        // assert
        equals1.Should().BeFalse();
        equals2.Should().BeFalse();
        equals3.Should().BeFalse();
        equals4.Should().BeFalse();
    }    
    
    [Fact]
    public void Should_Get_Distinct_Oracle_Tables()
    {
        // arrange
        var list = new List<OracleTable>
        {
            new()
            {
                SchemaName = "schema",
                TableName = "table",
                RowsCount = 5
            },
            new()
            {
                SchemaName = "schema",
                TableName = "table",
                RowsCount = 5
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OracleTable>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}