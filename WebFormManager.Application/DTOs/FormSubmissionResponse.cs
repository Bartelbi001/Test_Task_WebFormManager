namespace WebFormManager.Application.DTOs;

public record FormSubmissionResponse(string FormName, Dictionary<string, object> Data, DateTime SubmittedAt);