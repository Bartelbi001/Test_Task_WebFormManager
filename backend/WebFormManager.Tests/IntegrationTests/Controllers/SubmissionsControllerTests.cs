using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebFormManager.API.Interfaces;
using WebFormManager.Application.Contracts.Persistence;

namespace WebFormManager.Tests.IntegrationTests;

public class SubmissionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Mock<ISubmissionStorage> _storageMock = new();
    private readonly Mock<ISubmissionValidator> _validatorMock = new();
    
    public SubmissionsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_storageMock.Object);
                services.AddSingleton(_validatorMock.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task SubmitForm_ValidData_ReturnsCreated()
    {
        var jsonData = JsonSerializer.Serialize(new { name = "John", email = "john@example.com" });
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

        _validatorMock.Setup(v => v.Validate(It.IsAny<JsonElement>()));
        _storageMock.Setup(s => s.SaveAsync(It.IsAny<JsonElement>(), It.IsAny<CancellationToken>()));

        var response = await _client.PostAsync("api/submissions", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("message").GetString().Should().Be("Submission successfully saved.");
    }

    [Fact]
    public async Task SubmitForm_InvalidData_ReturnsBadRequest()
    {
        var invalidJson = JsonSerializer.Serialize(null as object);
        var content = new StringContent(invalidJson, System.Text.Encoding.UTF8, "application/json");

        _validatorMock.Setup(v => v.Validate(It.IsAny<JsonElement>()))
            .Throws(new ValidationException("The form data cannot be empty."));

        var response = await _client.PostAsync("api/submissions", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSubmissions_ReturnsList()
    {
        var mockData = new[] { JsonDocument.Parse("{\"name\": \"Alice\"}").RootElement };
        _storageMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(mockData.ToAsyncEnumerable());

        var response = await _client.GetAsync("api/submissions");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<JsonElement>>();

        result.Should().HaveCount(1);
        result[0].GetProperty("name").GetString().Should().Be("Alice");
    }

    [Fact]
    public async Task SearchSubmissions_ValidQuery_ReturnsResults()
    {
        var query = "Alice";
        var mockResults = new[] { JsonDocument.Parse("{\"name\": \"Alice\"}").RootElement };
        _storageMock.Setup(s => s.SearchAsync(query, It.IsAny<CancellationToken>()))
            .Returns(mockResults.ToAsyncEnumerable());

        var response = await _client.GetAsync($"api/submissions/search?query={query}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<JsonElement>>();

        result.Should().HaveCount(1);
        result[0].GetProperty("name").GetString().Should().Be("Alice");
    }

    [Fact]
    public async Task SearchSubmissions_EmptyQuery_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("api/submissions/search?query=");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}