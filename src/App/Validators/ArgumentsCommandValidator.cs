using App.Commands;
using FluentValidation;

namespace App.Validators;

public class ArgumentsCommandValidator : AbstractValidator<ArgumentsCommand>
{
    public ArgumentsCommandValidator()
    {
        RuleFor(x => x.DatabaseName)
            .NotEmpty();

        RuleFor(x => x.ProcedureName)
            .NotEmpty();
    }
}