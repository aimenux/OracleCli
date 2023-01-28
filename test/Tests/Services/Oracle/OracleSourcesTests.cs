using App.Services.Oracle;
using FluentAssertions;

namespace Tests.Services.Oracle;

public class OracleSourcesTests
{
    [Theory]
    [InlineData(1, "PROCEDURE GET_INFOS")]
    [InlineData(2, "PROCEDURE ADD_INFOS")]
    public void Should_Oracle_Sources_Be_Equals(int sourceLine, string sourceText)
    {
        // arrange
        var obj1 = new OracleSource
        {
            Line = sourceLine,
            Text = sourceText
        };
        var obj2 = new OracleSource
        {
            Line = sourceLine,
            Text = sourceText
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
    public void Should_Oracle_Sources_Not_Be_Equals()
    {
        // arrange
        var obj1 = new OracleSource
        {
            Line = 1,
            Text = "text",
        };
        
        var obj2 = new OracleSource
        {
            Line = 2,
            Text = "text",
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
    public void Should_Get_Distinct_Oracle_Sources()
    {
        // arrange
        var list = new List<OracleSource>
        {
            new()
            {
                Line = 1,
                Text = "PROCEDURE ADD_INFOS"
            },
            new()
            {
                Line = 1,
                Text = "PROCEDURE ADD_INFOS"
            },
            new(),
            new(),
            null
        };

        var hashset = new HashSet<OracleSource>(list);

        // act
        var hashsetCount = hashset.Count;
        var listDistinctCount = list.Distinct().Count();

        // assert
        hashsetCount.Should().Be(3);
        listDistinctCount.Should().Be(3);
    }
}