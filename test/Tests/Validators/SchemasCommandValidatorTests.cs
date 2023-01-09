using App.Commands;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class SchemasCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void SchemasCommand_Should_Be_Valid(SchemasCommand command)
    {
        // arrange
        var validator = new SchemasCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void SchemasCommand_Should_Not_Be_Valid(SchemasCommand command)
    {
        // arrange
        var validator = new SchemasCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<SchemasCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                FilterKeyword = null,
            });
            
            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                FilterKeyword = null,
            });
            
            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                FilterKeyword = null,
                MaxItems = 1
            });
            
            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                FilterKeyword = "keyword",
            });

            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                FilterKeyword = "keyword",
                MaxItems = 5000
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<SchemasCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = null,
                FilterKeyword = null,
            });
            
            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                FilterKeyword = null,
                MaxItems = 0
            });
            
            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                FilterKeyword = null,
                MaxItems = 5001
            });
            
            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                FilterKeyword = "a"
            });
            
            Add(new SchemasCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                FilterKeyword = "ab"
            });
        }
    }
}