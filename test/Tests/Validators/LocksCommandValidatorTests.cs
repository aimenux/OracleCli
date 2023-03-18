using App.Commands;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class LocksCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void LocksCommand_Should_Be_Valid(LocksCommand command)
    {
        // arrange
        var validator = new LocksCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void LocksCommand_Should_Not_Be_Valid(LocksCommand command)
    {
        // arrange
        var validator = new LocksCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<LocksCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new LocksCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null
            });
            
            Add(new LocksCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = "schema-name"
            });
            
            Add(new LocksCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = "schema-name",
                MinBlockingTimeInMinutes = 1
            });
            
            Add(new LocksCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = "schema-name",
                MinBlockingTimeInMinutes = 1,
                MaxItems = 1
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<LocksCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new LocksCommand(consoleService, oracleService, options)
            {
                DatabaseName = null,
                SchemaName = null,
            });
            
            Add(new LocksCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                MinBlockingTimeInMinutes = -1
            });
            
            Add(new LocksCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                MinBlockingTimeInMinutes = 3001
            });
            
            Add(new LocksCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                MaxItems = 0
            });
            
            Add(new LocksCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                MaxItems = 5001
            });
        }
    }
}