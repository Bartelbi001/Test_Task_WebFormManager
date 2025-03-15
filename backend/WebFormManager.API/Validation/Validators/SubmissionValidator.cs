using System.Text.Json;
using FluentValidation;
using WebFormManager.API.Interfaces;

namespace WebFormManager.API.Validation;

public class SubmissionValidator : AbstractValidator<JsonElement>, ISubmissionValidator
{
    public SubmissionValidator()
    {
        RuleFor(data => data.ValueKind)
            .Must(kind => kind is not JsonValueKind.Undefined and not JsonValueKind.Null)
            .WithMessage("The form data cannot be empty.");
    }

    public void Validate(JsonElement data)
    {
        var result = Validate(new ValidationContext<JsonElement>(data));
        
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}