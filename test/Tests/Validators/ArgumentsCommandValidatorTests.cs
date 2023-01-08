using App.Commands;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class ArgumentsCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void ArgumentsCommand_Should_Be_Valid(ArgumentsCommand command)
    {
        // arrange
        var validator = new ArgumentsCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void ArgumentsCommand_Should_Not_Be_Valid(ArgumentsCommand command)
    {
        // arrange
        var validator = new ArgumentsCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<ArgumentsCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);

            Add(new ArgumentsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                PackageName = "oracle-pkg",
                OwnerName = "owner"
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                PackageName = "oracle-pkg"
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc"
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<ArgumentsCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new ArgumentsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = null,
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, options)
            {
                DatabaseName = null,
                ProcedureName = "oracle-spc",
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, options)
            {
                DatabaseName = null,
                ProcedureName = null,
            });
        }
    }
}