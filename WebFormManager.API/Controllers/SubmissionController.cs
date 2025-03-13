using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebFormManager.API.Exceptions;
using WebFormManager.Application.Contracts.Persistence;
using WebFormManager.Application.DTOs;
using WebFormManager.Application.Mapping;

namespace WebFormManager.API.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionController : ControllerBase
{
    private readonly ISubmissionStorage _storage;
    private readonly IValidator<FormSubmissionRequest> _validator;

    public SubmissionController(ISubmissionStorage storage, IValidator<FormSubmissionRequest> validator)
    {
        _storage = storage;
        _validator = validator;
    }

    /// <summary>
    /// Submitting a new form
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] FormSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ApiValidationException(validationResult.Errors.First().ErrorMessage);
        }

        var submission = request.ToDomain();
        await _storage.SaveAsync(submission);

        return Ok(submission.ToResponse());
    }

    /// <summary>
    /// Getting a list of all submitted forms
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var submissions = await _storage.GetAllAsync();
        return Ok(submissions.ToResponseList());
    }
}