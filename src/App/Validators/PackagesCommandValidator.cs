using App.Commands;
using FluentValidation;

namespace App.Validators;

public class PackagesCommandValidator : AbstractValidator<PackagesCommand>
{
    public PackagesCommandValidator()
    {
        RuleFor(x => x.DatabaseName)
            .NotEmpty();
        
        RuleFor(x => x.MaxItems)
            .InclusiveBetween(1, 5000);
        
        RuleFor(x => x.FilterKeyword)
            .MinimumLength(2)
            .When(x => x.FilterKeyword is not null);
    }
}