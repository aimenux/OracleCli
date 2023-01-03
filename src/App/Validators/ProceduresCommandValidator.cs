﻿using App.Commands;
using FluentValidation;

namespace App.Validators;

public class ProceduresCommandValidator : AbstractValidator<ProceduresCommand>
{
    public ProceduresCommandValidator()
    {
        RuleFor(x => x.DatabaseName)
            .NotEmpty();
        
        RuleFor(x => x.MaxItems)
            .InclusiveBetween(1, 5000);

        RuleFor(x => x.Keyword)
            .MinimumLength(3)
            .When(x => x.Keyword is not null);
    }
}