using System.Text.Json;
using Microsoft.Extensions.Logging;
using Serilog;
using WebFormManager.Application.Contracts.Persistence;
using WebFormManager.Domain.Entities;
using WebFormManager.Infrastructure.Exceptions;

namespace WebFormManager.Infrastructure.Persistence;

public class FileSubmissionStorage : ISubmissionStorage
{
    private readonly string _filePath;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public FileSubmissionStorage(string filePath = "submissions.json")
    {
        _filePath = filePath;

        EnsureFileExists();
    }
    
    public async Task SaveAsync(FormSubmission submission)
    {
        Log.Information($"Saving file to: {Path.GetFullPath(_filePath)}");
        
        Log.Information("Waiting for semaphore...");
        await _semaphore.WaitAsync();
        Log.Information("Semaphore acquired!");
        
        try
        {
            var submissions = await GetAllAsync();
            submissions.Add(submission);

            await WriteToFileAsync(submissions);

            Log.Information("File submission {{@Submission}} saved to {{FilePath}}", submission, _filePath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save submission to file: {FilePath}", _filePath);
            throw new FileStorageException("Error writing to submissions file.", ex);
        }
        finally
        {
            _semaphore.Release();
            Log.Information("Semaphore released!");
        }
    }

    public async Task<List<FormSubmission>> GetAllAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            Log.Information($"Reading file from: {Path.GetFullPath(_filePath)}");
            
            string json = await File.ReadAllTextAsync(_filePath);
            return DeserializeSubmissions(json);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to read submissions from file: {FilePath}", _filePath);
            throw new FileStorageException("Error reading submissions file.", ex);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(_filePath))
        {
            try
            {
                File.WriteAllText(_filePath, "[]");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize storage file: {{FilePath}}", _filePath);
                throw new FileStorageException("Failed to initialize storage file.", ex);
            }
        }
    }

    private async Task WriteToFileAsync(List<FormSubmission> submissions)
    {
        string json = JsonSerializer.Serialize(submissions, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, json);
    }

    private List<FormSubmission> DeserializeSubmissions(string json)
    {
        return JsonSerializer.Deserialize<List<FormSubmission>>(json) ?? new List<FormSubmission>();
    }
}