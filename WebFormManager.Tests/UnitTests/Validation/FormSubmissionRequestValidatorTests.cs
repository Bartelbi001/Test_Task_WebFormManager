using FluentValidation.TestHelper;
using WebFormManager.Application.DTOs;
using WebFormManager.Application.Validation;

namespace WebFormManager.Tests.UnitTests.Validation;

public class FormSubmissionRequestValidatorTests
{
    private readonly FormSubmissionRequestValidator _validator;

    public FormSubmissionRequestValidatorTests()
    {
        _validator = new FormSubmissionRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Data_Is_Null()
    {
        // Arrange
        var request = new FormSubmissionRequest { Data = null };
        
        // Act
        var result = _validator.TestValidate(request);
        
        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Data)
            .WithErrorMessage("Submitted form cannot be null.");
    }

    [Fact]
    public void Should_Have_Error_When_Data_Is_Empty()
    {
        // Arrange
        var request = new FormSubmissionRequest { Data = new Dictionary<string, object>() };
        
        // Act
        var result = _validator.TestValidate(request);
        
        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Data)
            .WithErrorMessage("Submitted form cannot be empty.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Data_Is_Valid()
    {
        // Arrange
        var request = new FormSubmissionRequest
        {
            Data = new Dictionary<string, object> { { "Field1", "Value1" }}
        };
        
        // Act
        var result = _validator.TestValidate(request);
        
        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Data);
    }
}