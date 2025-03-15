using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebFormManager.API.Interfaces;
using WebFormManager.Application.Contracts.Persistence;

namespace WebFormManager.API.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionStorage _submissionStorage;
    private readonly ISubmissionValidator _submissionValidator;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public SubmissionsController(ISubmissionStorage submissionStorage, ISubmissionValidator submissionValidator)
    {
        _submissionStorage = submissionStorage ?? throw new ArgumentNullException(nameof(submissionStorage));
        _submissionValidator = submissionValidator ?? throw new ArgumentNullException(nameof(submissionValidator));
    }

    /// <summary>
    /// Submitting a new form.
    /// </summary>
    /// <response code="201">Submission successfully created</response>
    /// <response code="400">Validation failed</response>
    [HttpPost]
    [RequestSizeLimit(512 * 1024)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status413PayloadTooLarge)]
    public async Task<IActionResult> SubmitForm([FromBody] JsonElement data, CancellationToken cancellationToken)
    {
        _submissionValidator.Validate(data);
            
        Log.Information("Saving new submission.");
            
        await _submissionStorage.SaveAsync(data, cancellationToken);
            
        Log.Information("Submission successfully saved.");

        var response = new
        {
            message = "Submission successfully saved.",
            timestamp = DateTime.UtcNow
        };
            
        return Created(string.Empty, response);
    }
    
    /// <summary>
    /// Receiving all submitted forms.
    /// </summary>
    /// <response code="200">Returns a list of submissions (empty if none found)</response>
    /// [HttpGet]
    [HttpGet]
    [ProducesResponseType(typeof(List<JsonElement>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubmissions(CancellationToken cancellationToken)
    {
        Log.Information("Fetching all submissions.");

        var submissions = await _submissionStorage.GetAllAsync(cancellationToken).ToListAsync(cancellationToken); // ✅ Загружаем в список

        return Ok(submissions); // ✅ Гарантированно отдаём JSON
    }

    /// <summary>
    /// Searching submissions by query.
    /// </summary>
    /// <response code="200">Returns search results</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<JsonElement>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchSubmissions([FromQuery] string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query)) // ✅ Проверяем пустые строки
        {
            Log.Warning("Search query is empty");
            return BadRequest("Query parameter cannot be empty.");
        }

        Log.Information("Searching submissions for query: {Query}", query);

        var results = await _submissionStorage.SearchAsync(query, cancellationToken).ToListAsync(cancellationToken); // ✅ Загружаем в список

        return Ok(results); // ✅ Теперь API отдаёт корректный JSON-ответ
    }
}