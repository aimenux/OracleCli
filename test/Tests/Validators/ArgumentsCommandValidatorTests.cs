using App.Commands;
using App.Services.Console;
using App.Services.Exporters;
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
            var exportService = Substitute.For<ICSharpExportService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);

            Add(new ArgumentsCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                PackageName = "oracle-pkg",
                OwnerName = "owner"
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                PackageName = "oracle-pkg"
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                FunctionName = "oracle-fun",
                PackageName = "oracle-pkg"
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc"
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                FunctionName = "oracle-fun"
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<ArgumentsCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var exportService = Substitute.For<ICSharpExportService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new ArgumentsCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                FunctionName = "oracle-fun"
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = null,
                FunctionName = null
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = null,
                ProcedureName = "oracle-spc",
                FunctionName = null
            });
            
            Add(new ArgumentsCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = null,
                ProcedureName = null,
                FunctionName = null
            });
        }
    }
}