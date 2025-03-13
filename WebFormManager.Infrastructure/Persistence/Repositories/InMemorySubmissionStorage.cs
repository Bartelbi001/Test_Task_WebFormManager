using Serilog;
using WebFormManager.Application.Contracts.Persistence;
using WebFormManager.Domain.Entities;

namespace WebFormManager.Infrastructure.Persistence;

public class InMemorySubmissionStorage : ISubmissionStorage
{
    private readonly List<FormSubmission> _submissions = new();
    
    public Task SaveAsync(FormSubmission submission)
    {
        Log.Information("Saving file to InMemory.");
        
        _submissions.Add(submission);
        return Task.CompletedTask;
    }

    public Task<List<FormSubmission>> GetAllAsync()
    {
        Log.Information("Loading file from InMemory.");
        
        return Task.FromResult(_submissions.ToList());
    }
}