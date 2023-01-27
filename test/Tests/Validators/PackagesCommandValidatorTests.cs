using App.Commands;
using App.Services.Console;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class PackagesCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void PackagesCommand_Should_Be_Valid(PackagesCommand command)
    {
        // arrange
        var validator = new PackagesCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void PackagesCommand_Should_Not_Be_Valid(PackagesCommand command)
    {
        // arrange
        var validator = new PackagesCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<PackagesCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = null,
            });
            
            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = "owner",
                FilterKeyword = null,
            });
            
            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = null,
                MaxItems = 1
            });
            
            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = "keyword",
            });

            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = "owner",
                FilterKeyword = "keyword",
                MaxItems = 5000
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<PackagesCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = null,
                OwnerName = null,
                FilterKeyword = null,
            });
            
            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = null,
                MaxItems = 0
            });
            
            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = null,
                MaxItems = 5001
            });
            
            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = "a"
            });
            
            Add(new PackagesCommand(consoleService, oracleService, options)
            {
                DatabaseName = "oracle-for-tests",
                OwnerName = null,
                FilterKeyword = "ab",
                MaxItems = -1
            });
        }
    }
}