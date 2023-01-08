using App.Commands;
using App.Configuration;
using App.Services.Console;
using App.Services.Oracle;
using FluentAssertions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Commands;

public class FunctionsCommandTests
{
    [Fact]
    public async Task Should_FunctionsCommand_Return_Ok()
    {
        // arrange
        var app = new CommandLineApplication();
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);
        var command = new FunctionsCommand(consoleService, oracleService, options)
        {
            DatabaseName = "oracle-for-tests"
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ok);
    }
    
    [Fact]
    public async Task Should_FunctionsCommand_Return_Also_Ok()
    {
        // arrange
        var app = new CommandLineApplication();
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder()
            .WithDefaultDatabaseToUse("oracle-for-tests")
            .Build();
        var options = Options.Create(settings);
        var command = new FunctionsCommand(consoleService, oracleService, options);

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ok);
    }
    
    [Fact]
    public async Task Should_FunctionsCommand_Return_Ko()
    {
        // arrange
        var app = new CommandLineApplication();
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);
        var command = new FunctionsCommand(consoleService, oracleService, options)
        {
            DatabaseName = "oracle-for-tests",
            FilterKeyword = "#"
        };

        // act
        var result = await command.OnExecuteAsync(app);

        // assert
        result.Should().Be(Settings.ExitCode.Ko);
    }
}