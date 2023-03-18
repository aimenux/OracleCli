using App.Commands;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class SessionsCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void SessionsCommand_Should_Be_Valid(SessionsCommand command)
    {
        // arrange
        var validator = new SessionsCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void SessionsCommand_Should_Not_Be_Valid(SessionsCommand command)
    {
        // arrange
        var validator = new SessionsCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<SessionsCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new SessionsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null
            });
            
            Add(new SessionsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = "schema-name"
            });

            Add(new SessionsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = "schema-name",
                MaxItems = 1
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<SessionsCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new SessionsCommand(consoleService, oracleService, options)
            {
                DatabaseName = null,
                SchemaName = null,
            });
            
            Add(new SessionsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                MaxItems = 0
            });
            
            Add(new SessionsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                MaxItems = 5001
            });
        }
    }
}