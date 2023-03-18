using App.Commands;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class TablesCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void TablesCommand_Should_Be_Valid(TablesCommand command)
    {
        // arrange
        var validator = new TablesCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void TablesCommand_Should_Not_Be_Valid(TablesCommand command)
    {
        // arrange
        var validator = new TablesCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<TablesCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                TableName = null,
                FilterKeyword = null,
            });
            
            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = "schema",
                TableName = "table",
                FilterKeyword = null,
            });
            
            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                TableName = null,
                FilterKeyword = null,
                MaxItems = 1
            });
            
            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                TableName = null,
                FilterKeyword = "keyword",
            });

            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = "schema",
                TableName = "table",
                FilterKeyword = null,
                MaxItems = 5000
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<TablesCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = null,
                SchemaName = null,
                TableName = null,
                FilterKeyword = null,
            });
            
            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                TableName = null,
                FilterKeyword = null,
                MaxItems = 0
            });
            
            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                TableName = null,
                FilterKeyword = null,
                MaxItems = 5001
            });

            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                TableName = null,
                FilterKeyword = "a"
            });
            
            Add(new TablesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                SchemaName = null,
                TableName = null,
                FilterKeyword = "ab",
                MaxItems = -1
            });
        }
    }
}