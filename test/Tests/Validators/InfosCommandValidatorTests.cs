using App.Commands;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class InfosCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void InfosCommand_Should_Be_Valid(InfosCommand command)
    {
        // arrange
        var validator = new InfosCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void InfosCommand_Should_Not_Be_Valid(InfosCommand command)
    {
        // arrange
        var validator = new InfosCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<InfosCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new InfosCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests"
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<InfosCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new InfosCommand(consoleService, oracleService, options)
            {
                DatabaseName = null
            });
            
            Add(new InfosCommand(consoleService, oracleService, options)
            {
                DatabaseName = string.Empty,
            });
        }
    }
}