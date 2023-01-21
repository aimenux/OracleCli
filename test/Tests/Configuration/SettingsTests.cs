using App.Configuration;
using FluentAssertions;

namespace Tests.Configuration;

public class SettingsTests
{
    [Fact]
    public void Should_Get_Version()
    {
        // arrange
        // act
        var version = Settings.Cli.Version;

        // assert
        version.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public void Should_Get_Default_Working_Directory()
    {
        // arrange
        // act
        var directory = Settings.GetDefaultWorkingDirectory();

        // assert
        directory.Should().NotBeNullOrWhiteSpace();
    }
}