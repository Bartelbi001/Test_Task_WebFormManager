using System.ComponentModel;
using FluentValidation;
using WebFormManager.Domain.Entities;

namespace WebFormManager.Application.Validation;

public class FormSubmissionValidator : AbstractValidator<FormSubmission>
{
    public FormSubmissionValidator()
    {
        RuleFor(x => x.FormName)
            .NotEmpty().WithMessage("FormName is required.");
        
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("Form data cannot be empty.")
            .Must(data => data.Count > 0).WithMessage("At least one field must be present.");
    }
}