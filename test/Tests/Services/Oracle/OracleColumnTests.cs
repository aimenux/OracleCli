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
        var equals1 = obj1.Equals(obj1);
        var equals2 = obj1.Equals(obj2);
        var equals3 = obj1.Equals((object)obj2);

        // assert
        equals1.Should().BeTrue();
        equals2.Should().BeTrue();
        equals3.Should().BeTrue();
    }
    
    [Fact]
    public void Should_Oracle_Columns_Not_Be_Equals()
    {
        // arrange
        var obj1 = new OracleColumn
        {
            Name = "name",
            Type = "type",
            Position = 1,
            Nullable = "Y"
        };
        
        var obj2 = new OracleColumn
        {
            Name = "name",
            Type = "type",
            Position = 2,
            Nullable = "Y"
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