using App.Commands;
using FluentValidation;

namespace App.Validators;

public class InfosCommandValidator : AbstractValidator<InfosCommand>
{
    public InfosCommandValidator()
    {
        RuleFor(x => x.DatabaseName)
            .NotEmpty();
    }
}