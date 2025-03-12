using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebFormManager.Application.Contracts.Persistence;
using WebFormManager.Application.Validation;
using WebFormManager.Domain.Entities;
using WebFormManager.Domain.Exceptions;

namespace WebFormManager.API.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionController : ControllerBase
{
    private readonly ISubmissionStorage _storage;
    private readonly IValidator<FormSubmission> _validator;

    public SubmissionController(ISubmissionStorage storage, IValidator<FormSubmission> validator)
    {
        _storage = storage;
        _validator = validator;
    }

    /// <summary>
    /// Submitting a new form
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] FormSubmission submission, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(submission, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainValidationException(validationResult.Errors.First().ErrorMessage);
        }
        
        await _storage.SaveAsync(submission);
        return Ok(submission);
    }

    /// <summary>
    /// Getting a list of all submitted forms
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var submissions = await _storage.GetAllAsync();
        return Ok(submissions);
    }
}