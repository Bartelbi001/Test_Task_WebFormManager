using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Serilog;
using WebFormManager.Application.Contracts.Persistence;

namespace WebFormManager.Infrastructure.Persistence;

public class InMemorySubmissionStorage : ISubmissionStorage
{
    private readonly ConcurrentBag<JsonElement> _submissions = new(); // ✅ Потокобезопасная коллекция
    
    public Task SaveAsync(JsonElement submission, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); // ✅ Обрабатываем отмену

        Log.Information("Saving submission to InMemory storage.");
        
        _submissions.Add(submission);
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<JsonElement> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); // ✅ Обрабатываем отмену

        Log.Information("Loading submissions from InMemory storage.");
        
        foreach (var submission in _submissions)
        {
            cancellationToken.ThrowIfCancellationRequested(); // ✅ Проверяем отмену в цикле
            yield return submission;
            await Task.Yield(); // ✅ Позволяем обработку других задач в асинхронном контексте
        }
    }

    public async IAsyncEnumerable<JsonElement> SearchAsync(string query, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var submission in _submissions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (submission.ToString().Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                yield return submission;
                await Task.Yield();
            }
        }
    }
}