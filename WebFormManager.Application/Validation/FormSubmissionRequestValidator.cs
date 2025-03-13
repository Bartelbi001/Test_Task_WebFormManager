using FluentValidation;
using WebFormManager.Application.DTOs;

namespace WebFormManager.Application.Validation;

public class FormSubmissionRequestValidator : AbstractValidator<FormSubmissionRequest>
{
    public FormSubmissionRequestValidator()
    {
        RuleFor(x => x.Data)
            .NotNull().WithMessage("Submitted form cannot be null.")
            .NotEmpty().WithMessage("Submitted form cannot be empty.");
    }
}