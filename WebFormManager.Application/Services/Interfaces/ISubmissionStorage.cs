using System.Text.Json;

namespace WebFormManager.Application.Contracts.Persistence;

public interface ISubmissionStorage
{
    Task SaveAsync(JsonElement submission, CancellationToken cancellationToken);
    IAsyncEnumerable<JsonElement> GetAllAsync(CancellationToken cancellationToken);
    IAsyncEnumerable<JsonElement> SearchAsync(string query, CancellationToken cancellationToken);
}