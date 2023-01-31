using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleArgumentsTests
{
    [Theory]
    [InlineData("Argument1", 1, "DataType1", "IN")]
    [InlineData("Argument2", 2, "DataType2", "IN")]
    public void Should_Oracle_Arguments_Be_Equals(string name, int position, string dataType, string direction)
    {
        // arrange
        var obj1 = new OracleArgument
        {
            Name = name,
            Position = position,
            DataType = dataType,
            Direction = direction
        };
        var obj2 = new OracleArgument
        {
            Name = name,
            Position = position,
            DataType = dataType,
            Direction = direction
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
    public void Should_Oracle_Arguments_Not_Be_Equals()
    {
        // arrange
        var obj1 = new OracleArgument
        {
            Name = "name",
            Position = 1,
            DataType = "dataType",
            Direction = "direction"
        };
        
        var obj2 = new OracleArgument
        {
            Name = "name",
            Position = 2,
            DataType = "dataType",
            Direction = "direction"
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
    public void Should_Get_Distinct_Oracle_Arguments()
    {
        // arrange
        var list = new List<OracleArgument>
        {
            new()
            {
                Name = "name",
                Position = 1,
                DataType = "dataType",
                Direction = "direction"
            },
            new()
            {
                Name = "name",
                Position = 1,
                DataType = "dataType",
                Direction = "direction"
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OracleArgument>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}