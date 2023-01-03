using App.Commands;
using FluentValidation.Results;

namespace App.Validators;

public sealed class ValidationErrors : List<ValidationError>
{
    private ValidationErrors()
    {
    }

    private ValidationErrors(IEnumerable<ValidationError> errors) : base(errors)
    {
    }

    public static ValidationErrors New<TCommand>(IEnumerable<ValidationFailure> failures = null) where TCommand : AbstractCommand
    {
        if (failures is null)
        {
            return new ValidationErrors();
        }

        var errors = failures.Select(ValidationError.New<TCommand>);
        return new ValidationErrors(errors);
    }
}