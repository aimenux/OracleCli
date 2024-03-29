using App.Commands;
using FluentValidation;

namespace App.Validators;

public class ParametersCommandValidator : AbstractValidator<ParametersCommand>
{
    public ParametersCommandValidator()
    {
        RuleFor(x => x.DatabaseName)
            .NotEmpty();

        RuleFor(x => x.ProcedureName)
            .NotEmpty()
            .When(x => string.IsNullOrEmpty(x.FunctionName));

        RuleFor(x => x.ProcedureName)
            .Empty()
            .When(x => !string.IsNullOrEmpty(x.FunctionName));
        
        RuleFor(x => x.FunctionName)
            .NotEmpty()
            .When(x => string.IsNullOrEmpty(x.ProcedureName));
        
        RuleFor(x => x.FunctionName)
            .Empty()
            .When(x => !string.IsNullOrEmpty(x.ProcedureName));
    }
}