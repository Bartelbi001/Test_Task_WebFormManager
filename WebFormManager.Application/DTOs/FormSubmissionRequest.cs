namespace WebFormManager.Application.DTOs;

public record class FormSubmissionRequest
{
    public Dictionary<string, object> Data { get; init; } = new();
}
