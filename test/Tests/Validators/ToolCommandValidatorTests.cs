using App.Commands;
using App.Configuration;
using App.Exceptions;
using App.Services.Console;
using App.Services.Exporters;
using App.Services.Oracle;
using App.Validators;
using FluentAssertions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Validators;

public class ToolCommandValidatorTests
{
    [Fact]
    public void Should_Get_ValidationErrors_For_ToolCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);
        var command = new ToolCommand(consoleService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_ParametersCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var exportService = Substitute.For<ICSharpExportService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new ParametersCommand(consoleService, oracleService, exportService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_FunctionsCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new FunctionsCommand(consoleService, oracleService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_ObjectsCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new ObjectsCommand(consoleService, oracleService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_PackagesCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new PackagesCommand(consoleService, oracleService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_ProceduresCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new ProceduresCommand(consoleService, oracleService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_SchemasCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new SchemasCommand(consoleService, oracleService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_SourcesCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var exportService = Substitute.For<ISqlExportService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new SourcesCommand(consoleService, oracleService, exportService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_TablesCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new TablesCommand(consoleService, oracleService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_LocksCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new LocksCommand(consoleService, oracleService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Get_ValidationErrors_For_InfosCommand()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var oracleService = Substitute.For<IOracleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new InfosCommand(consoleService, oracleService, options);

        // act
        var validationErrors = ToolCommandValidator.Validate(command);

        // assert
        validationErrors.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Throw_Exception_For_Unexpected_Commands()
    {
        // arrange
        var consoleService = Substitute.For<IConsoleService>();
        var settings = new SettingsBuilder().Build();
        var options = Options.Create(settings);

        var command = new UnexpectedCommand(consoleService, options);

        // act
        var validationErrorsFunc = () => ToolCommandValidator.Validate(command);

        // assert
        validationErrorsFunc.Should().Throw<OracleCliException>();
    }

    private class UnexpectedCommand : AbstractCommand
    {
        public UnexpectedCommand(IConsoleService consoleService, IOptions<Settings> options) : base(consoleService, options)
        {
        }

        protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}