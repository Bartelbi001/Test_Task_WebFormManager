using WebFormManager.Domain.Entities;

namespace WebFormManager.Application.Contracts.Persistence;

public interface ISubmissionStorage
{
    Task SaveAsync(FormSubmission submission);
    Task<List<FormSubmission>> GetAllAsync();
}