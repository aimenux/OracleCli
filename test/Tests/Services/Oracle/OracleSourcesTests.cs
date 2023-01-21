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
}