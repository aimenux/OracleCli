using App.Commands;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class ProceduresCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void ProceduresCommand_Should_Be_Valid(ProceduresCommand command)
    {
        // arrange
        var validator = new ProceduresCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void ProceduresCommand_Should_Not_Be_Valid(ProceduresCommand command)
    {
        // arrange
        var validator = new ProceduresCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<ProceduresCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = null,
            });
            
            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = "owner",
                FilterKeyword = null,
            });
            
            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = null,
                MaxItems = 1
            });
            
            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = "keyword",
            });

            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = "owner",
                FilterKeyword = "keyword",
                MaxItems = 5000
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<ProceduresCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = null,
                OwnerName = null,
                FilterKeyword = null,
            });
            
            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = null,
                MaxItems = 0
            });
            
            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = null,
                MaxItems = 5001
            });
            
            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = "a"
            });
            
            Add(new ProceduresCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = "ab",
                MaxItems = -1
            });
        }
    }
}