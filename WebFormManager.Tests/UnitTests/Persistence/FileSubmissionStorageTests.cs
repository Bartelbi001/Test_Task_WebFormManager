using System.Text.Json;
using FluentAssertions;
using WebFormManager.Domain.Entities;
using WebFormManager.Infrastructure.Persistence;

namespace WebFormManager.Tests;

public class FileSubmissionStorageTests
{
    private readonly string _testFilePath;

    public FileSubmissionStorageTests()
    {
        _testFilePath = $"test_submissions_{Guid.NewGuid()}.json";
    }
    
    [Fact]
    public async Task SaveAsync_ShouldWriteSubmissionToFile()
    {
        // Arrange
        var storage = new FileSubmissionStorage(_testFilePath);
        var submission = new FormSubmission
        {
            FormName = "TestForm",
            Data = new Dictionary<string, object>
            {
                { "Name", "TestName" },
                { "Email", "ilya@example.com" },
            }
        };
        
        // Act
        await storage.SaveAsync(submission);
        var result = await storage.GetAllAsync();
        
        // Assert
        result.Should().ContainSingle().Which.FormName.Should().Be("TestForm");
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
    public async Task SaveAsync_ShouldAppendNewSubmission()
    {
        // Arrange
        var storage = new FileSubmissionStorage(_testFilePath);

        var submission1 = new FormSubmission { FormName = "Form1" };
        var submission2 = new FormSubmission { FormName = "Form2" };
        
        // Act
        await storage.SaveAsync(submission1);
        await storage.SaveAsync(submission2);
        
        var result = await storage.GetAllAsync();
        
        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        // Assert
        var storage = new FileSubmissionStorage("non_existing_file.json");
        
        // Act
        var result = await storage.GetAllAsync();
        
        // Assert
        result.Should().BeEmpty();
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
        
        File.WriteAllText(_testFilePath, JsonSerializer.Serialize(submissions));
        
        var storage = new FileSubmissionStorage(_testFilePath);
        
        // Act
        var result = await storage.GetAllAsync();
        
        // Assert
        result.Should().HaveCount(2);
        result[0].FormName.Should().Be("Form1");
        result[1].FormName.Should().Be("Form2");
    }

    [Fact]
    public async Task GetAllAsync_ShouldHandleInvalidJson()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "INVALID_JSON");
        
        var storage = new FileSubmissionStorage(_testFilePath);
        
        // Act
        var result = await storage.GetAllAsync();
        
        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenFileIsLocked()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "[]");

        using (var stream = File.Open(_testFilePath, FileMode.Open, FileAccess.Write, FileShare.None))
        {
            var storage = new FileSubmissionStorage(_testFilePath);
            
            // Act
            var result = await storage.GetAllAsync();
            
            // Assert
            result.Should().BeEmpty();
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
}