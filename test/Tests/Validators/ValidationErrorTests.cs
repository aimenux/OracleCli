using App.Commands;
using App.Validators;
using FluentAssertions;
using FluentValidation.Results;

namespace Tests.Validators;

public class ValidationErrorTests
{
    [Fact]
    public void Should_Get_OptionName_V1()
    {
        // arrange
        var validationFailure = new ValidationFailure(nameof(PackagesCommand.DatabaseName), "Required option");
        var validationError = ValidationError.New<PackagesCommand>(validationFailure);

        // act
        var optionName = validationError.OptionName();

        // assert
        optionName.Should().NotBeNullOrWhiteSpace();
        optionName.Should().Be("-d|--db");
    }
    
    [Fact]
    public void Should_Get_OptionName_V2()
    {
        // arrange
        var validationFailure = new ValidationFailure(nameof(ArgumentsCommand.ProcedureName), "Required option");
        var validationError = ValidationError.New<ArgumentsCommand>(validationFailure);

        // act
        var optionName = validationError.OptionName();

        // assert
        optionName.Should().NotBeNullOrWhiteSpace();
        optionName.Should().Be("-s|--spc");
    }
}