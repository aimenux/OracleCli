using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleTableTests
{
    [Theory]
    [InlineData("Owner1", "Table1", 10)]
    [InlineData("Owner2", "Table2", 20)]
    public void Should_Oracle_Tables_Be_Equals(string ownerName, string tableName, int rowsCount)
    {
        // arrange
        var obj1 = new OracleTable
        {
            OwnerName = ownerName,
            TableName = tableName,
            RowsCount = rowsCount
        };
        var obj2 = new OracleTable
        {
            OwnerName = ownerName,
            TableName = tableName,
            RowsCount = rowsCount
        };

        // act
        var equals = obj1.Equals(obj2);
        var objectsEquals = obj1.Equals((object)obj2);

        // assert
        equals.Should().BeTrue();
        objectsEquals.Should().BeTrue();
    }
    
    [Fact]
    public void Should_Get_Distinct_Oracle_Tables()
    {
        // arrange
        var list = new List<OracleTable>
        {
            new()
            {
                OwnerName = "owner",
                TableName = "table",
                RowsCount = 5
            },
            new()
            {
                OwnerName = "owner",
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