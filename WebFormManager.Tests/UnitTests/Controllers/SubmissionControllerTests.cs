using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebFormManager.API.Controllers;
using WebFormManager.Application.Contracts.Persistence;
using WebFormManager.Domain.Entities;

namespace WebFormManager.Tests.UnitTests.Controllers;

public class SubmissionControllerTests
{
    private readonly Mock<ISubmissionStorage> _storag/eMock;
    private readonly Mock<IValidator<FormSubmission>> _validatorMock;
    private readonly SubmissionController _controller;

    public SubmissionControllerTests()
    {
        _storageMock = new Mock<ISubmissionStorage>();
        _validatorMock = new Mock<IValidator<FormSubmission>>();

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<FormSubmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        
        _controller = new SubmissionController(_storageMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Sumbit_ShouldReturnOk_WhenValidDataProvided()
    {
        // Arrange
        var formData = new Dictionary<string, object>
        {
            { "FormName", "TestForm" },
            { "Field1", "Value1" }
        };
        
        // Act
        var result = await _controller.Submit(formData) as OkObjectResult;
    }
}