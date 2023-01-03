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
            ArgumentsCommand argumentsCommand => Validate(new ArgumentsCommandValidator(), argumentsCommand),
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