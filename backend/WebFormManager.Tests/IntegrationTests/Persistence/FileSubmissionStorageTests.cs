using System.Text.Json;
using FluentAssertions;
using WebFormManager.Infrastructure.Persistence;

namespace WebFormManager.Tests.IntegrationTests.Persistence;

public class FileSubmissionStorageTests : IAsyncLifetime
{
    private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "test_submissions.json");
    private FileSubmissionStorage _storage;

    public FileSubmissionStorageTests()
    {
        _storage = new FileSubmissionStorage(_testFilePath);
    }

    public async Task InitializeAsync()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    public async Task DisposeAsync()
    {
        await _storage.DisposeAsync();
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Fact]
    public async Task SaveAsync_ShouldStoreSubmission()
    {
        // Arrange
        var submission = JsonDocument.Parse("{ \"name\": \"Test User\" }").RootElement;
        
        // Act
        await _storage.SaveAsync(submission, CancellationToken.None);
        
        // Assert
        var submissions = await _storage.GetAllAsync(CancellationToken.None).ToListAsync();
        submissions.Should().ContainSingle().Which.ToString().Should().Contain("Test User");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSubmissions()
    {
        // Arrange
        var submission1 = JsonDocument.Parse("{ \"name\": \"User1\" }").RootElement;
        var submission2 = JsonDocument.Parse("{ \"name\": \"User2\" }").RootElement;
        await _storage.SaveAsync(submission1, CancellationToken.None);
        await _storage.SaveAsync(submission2, CancellationToken.None);
        
        // Act
        var submissions = await _storage.GetAllAsync(CancellationToken.None).ToListAsync();
        
        // Assert
        submissions.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingSubmissions()
    {
        // Arrange
        var submission1 = JsonDocument.Parse("{ \"message\": \"Hello world\", \"author\": \"John\" }").RootElement;
        var submission2 = JsonDocument.Parse("{ \"name\": \"Goodbye world\", \"author\": \"Alice\" }").RootElement;
        await _storage.SaveAsync(submission1, CancellationToken.None);
        await _storage.SaveAsync(submission2, CancellationToken.None);

        // Act
        var results = await _storage.SearchAsync("Hello", CancellationToken.None).ToListAsync();

        // Assert
        results.Should().ContainSingle(); // Должен вернуться ровно 1 результат
        results[0].ToString().Should().Contain("Hello world");
    }

    [Fact]
    public async Task SaveAsync_ShouldBeThreadSafe()
    {
        // Arrange
        var tasks = Enumerable.Range(0, 10).Select(async i =>
        {
            var submission = JsonDocument.Parse($"{{ \"id\": {i} }}").RootElement;
            await _storage.SaveAsync(submission, CancellationToken.None);
        });
        
        // Act
        await Task.WhenAll(tasks);
        var submissions = await _storage.GetAllAsync(CancellationToken.None).ToListAsync();
        
        // Assert
        submissions.Should().HaveCount(10);
    }
}