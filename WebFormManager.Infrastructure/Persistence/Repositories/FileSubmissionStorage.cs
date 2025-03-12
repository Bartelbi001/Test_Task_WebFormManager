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
    private readonly object _lock = new();

    public FileSubmissionStorage(string filePath = "submissions.json")
    {
        _filePath = filePath;
    }
    
    public async Task SaveAsync(FormSubmission submission)
    {
        List<FormSubmission> submissions = await GetAllAsync();
        submissions.Add(submission);
        
        string json = JsonSerializer.Serialize(submissions, new JsonSerializerOptions { WriteIndented = true });

        lock (_lock)
        {
            File.WriteAllText(_filePath, json);
        }
        
        Log.Information("File submission {@Submission} saved to {FilePath}", submission, _filePath);
    }

    public async Task<List<FormSubmission>> GetAllAsync()
    {
        if(!File.Exists(_filePath))
            return new List<FormSubmission>();

        try
        {
            lock (_lock)
            {
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<FormSubmission>>(json) ?? new List<FormSubmission>();
            }
        }
        catch (Exception ex)
        {
            throw new FileStorageException("Error reading submissions file.", ex);
        }
    }
}