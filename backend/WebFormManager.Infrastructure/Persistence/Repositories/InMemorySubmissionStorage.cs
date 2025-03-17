using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Serilog;
using WebFormManager.Application.Contracts.Persistence;

namespace WebFormManager.Infrastructure.Persistence;

public class InMemorySubmissionStorage : ISubmissionStorage
{
    private readonly ConcurrentBag<JsonElement> _submissions = new();
    
    public Task SaveAsync(JsonElement submission, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Log.Information("Saving submission to InMemory storage.");
        
        _submissions.Add(submission);
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<JsonElement> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Log.Information("Loading submissions from InMemory storage.");
        
        foreach (var submission in _submissions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return submission;
            await Task.Yield();
        }
    }

    public async IAsyncEnumerable<JsonElement> SearchAsync(string query, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            yield break;

        query = Uri.UnescapeDataString(query).Trim();

        foreach (var submission in _submissions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (ContainsQuery(submission, query))
            {
                yield return submission;
                await Task.Yield();
            }
        }
    }
    
    private static bool ContainsQuery(JsonElement submission, string query)
    {
        foreach (var property in submission.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.String)
            {
                var value = property.Value.GetString();
                if (!string.IsNullOrEmpty(value) && value.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }
}