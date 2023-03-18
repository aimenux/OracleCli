using App.Commands;
using App.Services.Console;
using App.Services.Exporters;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class SourcesCommandValidatorTests
{
    [Theory]
    [ClassData(typeof(ValidOracleTestCases))]
    public void SourcesCommand_Should_Be_Valid(SourcesCommand command)
    {
        // arrange
        var validator = new SourcesCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [ClassData(typeof(NotValidOracleTestCases))]
    public void SourcesCommand_Should_Not_Be_Valid(SourcesCommand command)
    {
        // arrange
        var validator = new SourcesCommandValidator();

        // act
        var result = validator.Validate(command);

        // assert
        result.IsValid.Should().BeFalse();
    }
    
    private class ValidOracleTestCases : TheoryData<SourcesCommand>
    {
        public ValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var exportService = Substitute.For<ISqlExportService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);

            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                PackageName = "oracle-pkg",
                SchemaName = "schema"
            });
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                PackageName = "oracle-pkg"
            });
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                FunctionName = "oracle-fun",
                PackageName = "oracle-pkg"
            });
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc"
            });
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                FunctionName = "oracle-fun"
            });
        }
    }
    
    private class NotValidOracleTestCases : TheoryData<SourcesCommand>
    {
        public NotValidOracleTestCases()
        {
            var consoleService = Substitute.For<IConsoleService>();
            var oracleService = Substitute.For<IOracleService>();
            var exportService = Substitute.For<ISqlExportService>();
            var settings = new SettingsBuilder().Build();
            var options = Options.Create(settings);
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                FunctionName = "oracle-fun"
            });
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = null,
                FunctionName = null
            });
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = null,
                ProcedureName = "oracle-spc",
            });
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = null,
                ProcedureName = null,
                FunctionName = null
            });
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                ProcedureName = "oracle-spc",
                OutputDirectory = null
            });
            
            Add(new SourcesCommand(consoleService, oracleService, exportService, options)
            {
                DatabaseName = "oracle-for-tests",
                FunctionName = "oracle-fun",
                OutputDirectory = null
            });
        }
    }
}