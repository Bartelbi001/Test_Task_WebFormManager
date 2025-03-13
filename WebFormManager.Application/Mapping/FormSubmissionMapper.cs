using WebFormManager.Application.DTOs;
using WebFormManager.Domain.Entities;

namespace WebFormManager.Application.Mapping;

public static class FormSubmissionMapper
{
    public static FormSubmission ToDomain(this FormSubmissionRequest request)
    {
        return new FormSubmission()
        {
            FormName = request.Data.TryGetValue("FormName", out var formName) ? formName?.ToString() ?? "UnknownForm" : "UnknownForm",
            Data = request.Data
        };
    }

    public static FormSubmissionResponse ToResponse(this FormSubmission submission)
    {
        return new FormSubmissionResponse(
            FormName: submission.FormName,
            Data: submission.Data,
            SubmittedAt: submission.SubmittedAt
            );
    }

    public static List<FormSubmissionResponse> ToResponseList(this IEnumerable<FormSubmission> submissions)
    {
        return submissions.Select(s => s.ToResponse()).ToList();
    }
}