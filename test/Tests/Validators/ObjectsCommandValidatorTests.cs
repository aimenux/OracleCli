using App.Commands;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class ObjectsCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void ObjectsCommand_Should_Be_Valid(ObjectsCommand command)
    {
        // arrange
        var validator = new ObjectsCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void ObjectsCommand_Should_Not_Be_Valid(ObjectsCommand command)
    {
        // arrange
        var validator = new ObjectsCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<ObjectsCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                FilterKeyword = null,
            });
            
            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = "schema",
                FilterKeyword = null,
            });
            
            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                FilterKeyword = null,
                MaxItems = 1
            });
            
            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                FilterKeyword = "keyword",
            });

            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = "schema",
                FilterKeyword = "keyword",
                MaxItems = 5000
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<ObjectsCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = null,
                SchemaName = null,
                FilterKeyword = null,
            });
            
            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                FilterKeyword = null,
                MaxItems = 0
            });
            
            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                FilterKeyword = null,
                MaxItems = 5001
            });
            
            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                FilterKeyword = "a"
            });
            
            Add(new ObjectsCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                FilterKeyword = "ab",
                MaxItems = -1
            });
        }
    }
}