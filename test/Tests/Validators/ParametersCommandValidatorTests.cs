using App.Commands;
using App.Services.Console;
using App.Services.Exporters;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class ParametersCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void ParametersCommand_Should_Be_Valid(ParametersCommand command)
    {
        // arrange
        var validator = new ParametersCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void ParametersCommand_Should_Not_Be_Valid(ParametersCommand command)
    {
        // arrange
        var validator = new ParametersCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<ParametersCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var exportService = Substitute.For<ICSharpExportService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);

            Add(new ParametersCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                PackageName = "oracle-pkg",
                OwnerName = "owner"
            });
            
            Add(new ParametersCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                PackageName = "oracle-pkg"
            });
            
            Add(new ParametersCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                FunctionName = "oracle-fun",
                PackageName = "oracle-pkg"
            });
            
            Add(new ParametersCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc"
            });
            
            Add(new ParametersCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                FunctionName = "oracle-fun"
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<ParametersCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var exportService = Substitute.For<ICSharpExportService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new ParametersCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                FunctionName = "oracle-fun"
            });
            
            Add(new ParametersCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = null,
                FunctionName = null
            });
            
            Add(new ParametersCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = null,
                ProcedureName = "oracle-spc",
                FunctionName = null
            });
            
            Add(new ParametersCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = null,
                ProcedureName = null,
                FunctionName = null
            });
        }
    }
}