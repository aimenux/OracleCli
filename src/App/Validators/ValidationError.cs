using App.Commands;
using App.Extensions;
using FluentValidation.Results;
using McMaster.Extensions.CommandLineUtils;

namespace App.Validators;

public sealed class ValidationError
{
    private ValidationError(Type commandType, ValidationFailure failure)
    {
        CommandType = commandType;
        Failure = failure;
    }

    public Type CommandType { get; }

    public ValidationFailure Failure { get; }

    public string OptionName()
    {
        var propertyInfo = CommandType
            .GetProperties()
            .Single(x => x.Name.IgnoreEquals(Failure.PropertyName));

        var optionAttribute = propertyInfo
            .GetCustomAttributes(true)
            .OfType<OptionAttribute>()
            .Single();

        return optionAttribute.Template;
    }

    public static ValidationError New<TCommand>(ValidationFailure failure) where TCommand : AbstractCommand
    {
        return new ValidationError(typeof(TCommand), failure);
    }
}