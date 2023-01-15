using App.Commands;
using FluentValidation;

namespace App.Validators;

public class SourcesCommandValidator : AbstractValidator<SourcesCommand>
{
    public SourcesCommandValidator()
    {
        RuleFor(x => x.DatabaseName)
            .NotEmpty();

        RuleFor(x => x.ProcedureName)
            .NotEmpty();

        RuleFor(x => x.OutputDirectory)
            .NotEmpty()
            .Must(Directory.Exists);
    }
}