using App.Commands;
using FluentValidation;

namespace App.Validators;

public class LocksCommandValidator : AbstractValidator<LocksCommand>
{
    public LocksCommandValidator()
    {
        RuleFor(x => x.DatabaseName)
            .NotEmpty();
        
        RuleFor(x => x.MinBlockingTimeInMinutes)
            .InclusiveBetween(0, 3000);
        
        RuleFor(x => x.MaxItems)
            .InclusiveBetween(1, 5000);
    }
}