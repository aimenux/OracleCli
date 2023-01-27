using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleColumnTests
{
    [Theory]
    [InlineData("Column1", "Type1", 1, "N")]
    [InlineData("Column2", "Type2", 2, "Y")]
    public void Should_Oracle_Columns_Be_Equals(string name, string type, int position, string nullable)
    {
        // arrange
        var obj1 = new OracleColumn
        {
            Name = name,
            Type = type,
            Position = position,
            Nullable = nullable
        };
        var obj2 = new OracleColumn
        {
            Name = name,
            Type = type,
            Position = position,
            Nullable = nullable
        };

        // act
        var equals = obj1.Equals(obj2);
        var objectsEquals = obj1.Equals((object)obj2);

        // assert
        equals.Should().BeTrue();
        objectsEquals.Should().BeTrue();
    }
    
    [Fact]
    public void Should_Get_Distinct_Oracle_Columns()
    {
        // arrange
        var list = new List<OracleColumn>
        {
            new()
            {
                Name = "column",
                Type = "type",
                Position = 1,
                Nullable = "N"
            },
            new()
            {
                Name = "column",
                Type = "type",
                Position = 1,
                Nullable = "N"
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OracleColumn>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}