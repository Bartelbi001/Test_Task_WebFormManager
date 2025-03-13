using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebFormManager.API.Controllers;
using WebFormManager.Application.Contracts.Persistence;
using WebFormManager.Application.DTOs;
using WebFormManager.Application.Mapping;
using WebFormManager.Domain.Entities;

namespace WebFormManager.Tests.UnitTests.Controllers;

public class SubmissionControllerTests
{
    private readonly Mock<ISubmissionStorage> _storageMock;
    private readonly Mock<IValidator<FormSubmissionRequest>> _validatorMock;
    private readonly SubmissionController _controller;

    public SubmissionControllerTests()
    {
        _storageMock = new Mock<ISubmissionStorage>();
        _validatorMock = new Mock<IValidator<FormSubmissionRequest>>();

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<FormSubmissionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());


        _controller = new SubmissionController(_storageMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Submit_ShouldReturnOk_WhenValidDataProvided()
    {
        // Arrange
        var request = new FormSubmissionRequest
        {
            Data = new Dictionary<string, object>
            {
                { "FormName", "TestForm" },
                { "Field1", "Value1" }
            }
        };

        var expectedSubmission = FormSubmissionMapper.ToDomain(request);
        var expectedResponse = FormSubmissionMapper.ToResponse(expectedSubmission);

        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _storageMock
            .Setup(s => s.SaveAsync(expectedSubmission))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Submit(request, CancellationToken.None) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var actualResponse = result.Value.As<FormSubmissionResponse>();
        actualResponse.Should().NotBeNull();
        
        actualResponse.SubmittedAt.Should().BeCloseTo(expectedResponse.SubmittedAt, TimeSpan.FromMilliseconds(100));
        
        _storageMock.Verify(s => s.SaveAsync(It.Is<FormSubmission>(
                submission => submission.FormName == expectedSubmission.FormName 
                              && submission.Data.SequenceEqual(expectedSubmission.Data))),
            Times.Once);
    }
}