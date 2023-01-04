using App.Commands;
using FluentValidation;

namespace App.Validators;

public class FunctionsCommandValidator : AbstractValidator<FunctionsCommand>
{
    public FunctionsCommandValidator()
    {
        RuleFor(x => x.DatabaseName)
            .NotEmpty();
        
        RuleFor(x => x.MaxItems)
            .InclusiveBetween(1, 5000);

        RuleFor(x => x.FilterKeyword)
            .MinimumLength(3)
            .When(x => x.FilterKeyword is not null);
    }
}