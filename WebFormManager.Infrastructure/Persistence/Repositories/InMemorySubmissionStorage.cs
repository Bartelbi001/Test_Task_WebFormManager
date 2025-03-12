using WebFormManager.Application.Contracts.Persistence;
using WebFormManager.Domain.Entities;

namespace WebFormManager.Infrastructure.Persistence;

public class InMemorySubmissionStorage : ISubmissionStorage
{
    private readonly List<FormSubmission> _submissions = new();
    
    public Task SaveAsync(FormSubmission submission)
    {
        _submissions.Add(submission);
        return Task.CompletedTask;
    }

    public Task<List<FormSubmission>> GetAllAsync()
    {
        return Task.FromResult(_submissions.ToList());
    }
}