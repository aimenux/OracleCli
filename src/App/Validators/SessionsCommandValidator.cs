using App.Commands;
using FluentValidation;

namespace App.Validators;

public class SessionsCommandValidator : AbstractValidator<SessionsCommand>
{
    public SessionsCommandValidator()
    {
        RuleFor(x => x.DatabaseName)
            .NotEmpty();
        
        RuleFor(x => x.MaxItems)
            .InclusiveBetween(1, 5000);
    }
}