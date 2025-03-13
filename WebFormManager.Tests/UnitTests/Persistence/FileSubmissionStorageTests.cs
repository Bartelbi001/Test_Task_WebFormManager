using System.Text.Json;
using FluentAssertions;
using WebFormManager.Domain.Entities;
using WebFormManager.Infrastructure.Persistence;

namespace WebFormManager.Tests;

public class FileSubmissionStorageTests : IDisposable
{
    private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "test_submissions.json");
    private readonly FileSubmissionStorage _storage;

    public FileSubmissionStorageTests()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);

        _storage = new FileSubmissionStorage(_testFilePath);
    }
    
    [Fact]
    public async Task SaveAsync_ShouldAddSubmissionToFile()
    {
        // Arrange
        var submission = new FormSubmission
        {
            FormName = "TestForm",
            Data = new Dictionary<string, object> { { "Field1", "Value1" } },
            SubmittedAt = DateTime.UtcNow
        };
        
        // Act
        await _storage.SaveAsync(submission);
        var result = await _storage.GetAllAsync();
        
        // Assert
        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(submission, options =>
                options.Excluding(x => x.SubmittedAt)
                    .Using<DateTime>(ctx => 
                        ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                    .WhenTypeIs<DateTime>());
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnStoredSubmissions()
    {
        // Arrange
        var submissions = new List<FormSubmission>
        {
            new() { FormName = "Form1", Data = new Dictionary<string, object> { { "Field1", "Value1" } } },
            new() { FormName = "Form2", Data = new Dictionary<string, object> { { "Field2", "Value2" } } },
        };
        
       await File.WriteAllTextAsync(_testFilePath, JsonSerializer.Serialize(submissions));
        
        // Act
        var result = await _storage.GetAllAsync();
        
        // Assert
        result.Should().BeEquivalentTo(submissions);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        // Assert
        await File.WriteAllTextAsync(_testFilePath, "");
        
        // Act
        var result = await _storage.GetAllAsync();
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task SaveAsync_ShouldNotThrow_WhenFileIsLocked()
    {
        // Arrange
        using (var stream = File.Open(_testFilePath, FileMode.Create, FileAccess.Read, FileShare.None))
        {
            var storage = new FileSubmissionStorage(_testFilePath);
            var submission = new FormSubmission { FormName = "LockedFileTest" };
            
            // Act
            Func<Task> action = async () => await storage.SaveAsync(submission);
            
            // Assert
            await action.Should().NotThrowAsync();
        }
    }

    [Fact]
    public void Storage_ShouldCreateFileIfNotExists()
    {
        // Arrange
        if(File.Exists(_testFilePath))
            File.Delete(_testFilePath);
        
        // Act
        var storage = new FileSubmissionStorage(_testFilePath);
        var exists = File.Exists(_testFilePath);
        
        // Assert
        exists.Should().BeTrue();
    }

    public void Dispose()
    {
        if(File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }
}