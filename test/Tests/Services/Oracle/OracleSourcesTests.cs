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
        var source1 = new OracleSource
        {
            Line = sourceLine,
            Text = sourceText
        };
        var source2 = new OracleSource
        {
            Line = sourceLine,
            Text = sourceText
        };

        // act
        var sourcesEquals = source1.Equals(source2);
        var objectsEquals = source1.Equals((object)source2);

        // assert
        sourcesEquals.Should().BeTrue();
        objectsEquals.Should().BeTrue();
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