using App.Commands;
using FluentValidation;

namespace App.Validators;

public static class ToolCommandValidator
{
    public static ValidationErrors Validate<TCommand>(TCommand command) where TCommand : AbstractCommand
    {
        return command switch
        {
            ToolCommand _ => ValidationErrors.New<ToolCommand>(),
            InfosCommand infosCommand => Validate(new InfosCommandValidator(), infosCommand),
            LocksCommand locksCommand => Validate(new LocksCommandValidator(), locksCommand),
            TablesCommand tablesCommand => Validate(new TablesCommandValidator(), tablesCommand),
            ObjectsCommand objectsCommand => Validate(new ObjectsCommandValidator(), objectsCommand),
            SchemasCommand schemasCommand => Validate(new SchemasCommandValidator(), schemasCommand),
            SourcesCommand sourcesCommand => Validate(new SourcesCommandValidator(), sourcesCommand),
            SessionsCommand sessionsCommand => Validate(new SessionsCommandValidator(), sessionsCommand),
            ArgumentsCommand argumentsCommand => Validate(new ArgumentsCommandValidator(), argumentsCommand),
            FunctionsCommand functionsCommand => Validate(new FunctionsCommandValidator(), functionsCommand),
            ProceduresCommand proceduresCommand => Validate(new ProceduresCommandValidator(), proceduresCommand),
            PackagesCommand packagesCommand => Validate(new PackagesCommandValidator(), packagesCommand),
            _ => throw new ArgumentOutOfRangeException(nameof(command), typeof(TCommand), "Unexpected command type")
        };
    }

    private static ValidationErrors Validate<TCommand>(IValidator<TCommand> validator, TCommand command) where TCommand : AbstractCommand
    {
        var errors = validator
            .Validate(command)
            .Errors;
        return ValidationErrors.New<TCommand>(errors);
    }
}