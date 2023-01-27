using App.Extensions;
using FluentAssertions;

namespace Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData("abc", "abc")]
    [InlineData("abc", "Abc")]
    [InlineData("abc", "aBc")]
    [InlineData("abc", "abC")]
    [InlineData("abc", "ABC")]
    public void Should_Be_Equals(string left, string right)
    {
        // arrange
        // act
        var areEquals = left.IgnoreEquals(right);

        // assert
        areEquals.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("", null)]
    [InlineData(null, "")]
    [InlineData("abc", "âbc")]
    [InlineData("edf", "èdf")]
    [InlineData("uvw", "ùvw")]
    public void Should_Not_Be_Equals(string left, string right)
    {
        // arrange
        // act
        var areEquals = left.IgnoreEquals(right);

        // assert
        areEquals.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("abcde", "bcd")]
    [InlineData("abcde", "bCd")]
    [InlineData("ABCDE", "bcd")]
    public void Should_Contains(string left, string right)
    {
        // arrange
        // act
        var contains = left.IgnoreContains(right);

        // assert
        contains.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("abcde", "abd")]
    [InlineData("abcde", "abe")]
    [InlineData("abcde", "bce")]
    public void Should_Not_Contains(string left, string right)
    {
        // arrange
        // act
        var contains = left.IgnoreContains(right);

        // assert
        contains.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("abc")]
    [InlineData("efg")]
    public void Should_Array_Contains(string input)
    {
        // arrange
        var array = new[]
        {
            "ABC",
            "EFG"
        };
        
        // act
        var contains = array.IgnoreContains(input);

        // assert
        contains.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("abc")]
    [InlineData("efg")]
    public void Should_Array_Not_Contains(string input)
    {
        // arrange
        var array = new[]
        {
            "XYZ",
            "UVW"
        };
        
        // act
        var contains = array.IgnoreContains(input);

        // assert
        contains.Should().BeFalse();
    }
}