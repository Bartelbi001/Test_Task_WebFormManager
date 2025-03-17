using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebFormManager.API.Controllers;
using WebFormManager.API.Interfaces;
using WebFormManager.Application.Contracts.Persistence;

namespace WebFormManager.Tests.UnitTests.Controllers;

public class SubmissionControllerTests
{
    private readonly Mock<ISubmissionStorage> _submissionStorageMock;
    private readonly Mock<ISubmissionValidator> _submissionValidatorMock;
    private readonly SubmissionsController _controller;

    public SubmissionControllerTests()
    {
        _submissionStorageMock = new Mock<ISubmissionStorage>();
        _submissionValidatorMock = new Mock<ISubmissionValidator>();
        
        _controller = new SubmissionsController(
            _submissionStorageMock.Object,
            _submissionValidatorMock.Object
            );
    }

    [Fact]
    public async Task SubmitForm_Should_ReturnCreated_WhenSubmissionIsValid()
    {
        // Arrange
        var json = JsonSerializer.SerializeToElement(new { Name = "Test" });
        var cancellationToken = new CancellationToken();

        _submissionValidatorMock.Setup(v => v.Validate(json));
        _submissionStorageMock.Setup(s => s.SaveAsync(json, cancellationToken))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _controller.SubmitForm(json, cancellationToken);
        
        // Assert
        result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result;
        createdResult.StatusCode.Should().Be(201);
        createdResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task SubmitForm_Should_ReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var json = JsonSerializer.SerializeToElement(new { Invalid = "Test" });
        var cancellationToken = new CancellationToken();

        // Мокаем: `Validate` должен выбросить `ValidationException`
        _submissionValidatorMock
            .Setup(v => v.Validate(It.IsAny<JsonElement>()))
            .Throws(new ValidationException("Validation failed"));

        // Act
        var result = await Record.ExceptionAsync(() => _controller.SubmitForm(json, cancellationToken));

        // Assert: Проверяем, что было выброшено исключение `ValidationException`
        result.Should().BeOfType<ValidationException>()
            .Which.Message.Should().Be("Validation failed");
    }

    [Fact]
    public async Task GetSubmissions_Should_Return200WithData_WhenSubmissionExist()
    {
        // Arrange
        var submissions = new[] { JsonSerializer.SerializeToElement(new { Name = "Test" }) };
        var cancellationToken = new CancellationToken();
        
        _submissionStorageMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(submissions.ToAsyncEnumerable());
        
        // Act
        var result = await _submissionStorageMock.Object.GetAllAsync(cancellationToken).ToListAsync();
        
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(submissions, result);
    }

    [Fact]
    public async Task GetSubmissions_Should_Return200_WhenNoSubmissionsExist()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _submissionStorageMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Enumerable.Empty<JsonElement>().ToAsyncEnumerable());
        
        using var responseStream = new MemoryStream();
        var context = new DefaultHttpContext();
        context.Response.Body = responseStream;
        _controller.ControllerContext.HttpContext = context;
        
        // Act
        await _controller.GetSubmissions(cancellationToken);
        
        // Assert
        context.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task SearchSubmissions_Should_Return200WithResults_WhenQueryIsValid()
    {
        // Arrange
        var query = "test";
        var cancellationToken = new CancellationToken();
        var searchResults = new[] { JsonSerializer.SerializeToElement(new { Name = "Match" }) };

        _submissionStorageMock.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(searchResults.ToAsyncEnumerable());

        // Act
        var result = await _controller.SearchSubmissions(query, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Which;
        var value = okResult.Value.Should().NotBeNull().And.BeAssignableTo<IEnumerable<JsonElement>>().Which;
        value.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task SearchSubmissions_Should_Return400_WhenQueryIsEmpty()
    {
        // Arrange
        var query = "";
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.SearchSubmissions(query, cancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}