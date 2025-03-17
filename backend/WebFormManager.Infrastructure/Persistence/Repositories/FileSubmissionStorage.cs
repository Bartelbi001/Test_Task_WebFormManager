using System.Runtime.CompilerServices;
using System.Text.Json;
using Serilog;
using WebFormManager.Application.Contracts.Persistence;
using WebFormManager.Infrastructure.Exceptions;

namespace WebFormManager.Infrastructure.Persistence;

public class FileSubmissionStorage : ISubmissionStorage, IAsyncDisposable
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 100;

    public FileSubmissionStorage(string filePath = "submissions.json")
    {
        _filePath = filePath;
        EnsureFileExists();
    }

    public async Task SaveAsync(JsonElement submission, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            Log.Information("Saving submission to {FilePath}", _filePath);
            
            var submissions = new List<JsonElement>();

            await foreach (var item in ReadFromFileAsync(cancellationToken))
            {
                submissions.Add(item);
            }
            
            submissions.Add(submission);
            await WriteToFileAsync(submissions, cancellationToken);

            Log.Information("Submission saved successfully to {FilePath}", _filePath);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async IAsyncEnumerable<JsonElement> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var item in ReadFromFileAsync(cancellationToken))
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<JsonElement> SearchAsync(string query, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Log.Information("Searching in {FilePath} for query: {Query}", _filePath, query);

        if (string.IsNullOrWhiteSpace(query))
        {
            yield break;
        }

        query = Uri.UnescapeDataString(query).Trim();

        await foreach (var submission in ReadFromFileAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (ContainsQuery(submission, query))
            {
                yield return submission;
                await Task.Yield();
            }
        }
    }

    private void EnsureFileExists()
    {
        string? directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
        {
            File.WriteAllText(_filePath, "[]");
            Log.Information("Initialized empty submission storage: {FilePath}", _filePath);
        }
    }

    private async IAsyncEnumerable<JsonElement> ReadFromFileAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            Log.Warning("File {FilePath} does not exist. Returning empty collection.", _filePath);
            yield break;
        }

        await using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        using var jsonDocument = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (jsonDocument.RootElement.ValueKind != JsonValueKind.Array)
        {
            Log.Error("Invalid JSON format in {FilePath}. Expected an array.", _filePath);
            yield break;
        }

        foreach (var element in jsonDocument.RootElement.EnumerateArray())
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return element.Clone();
            await Task.Yield();
        }
    }
    
    private async Task WriteToFileAsync(List<JsonElement> submissions, CancellationToken cancellationToken)
    {
        string tempFilePath = $"{_filePath}.tmp";

        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using var memoryStream = new MemoryStream();
                await JsonSerializer.SerializeAsync(memoryStream, submissions, _jsonOptions, cancellationToken);
                await File.WriteAllBytesAsync(tempFilePath, memoryStream.ToArray(), cancellationToken);

                File.Move(tempFilePath, _filePath, overwrite: true);

                Log.Information("File {FilePath} successfully updated.", _filePath);
                return;
            }
            catch (IOException ex) when (attempt < MaxRetries - 1)
            {
                Log.Warning(ex, "File {FilePath} is locked. Retrying in {Delay}ms...", _filePath, RetryDelayMs);
                await Task.Delay(RetryDelayMs, cancellationToken);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        throw new FileStorageException("Unable to write to file after multiple retries.");
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

    public ValueTask DisposeAsync()
    {
        _semaphore.Dispose();
        return ValueTask.CompletedTask;
    }
}