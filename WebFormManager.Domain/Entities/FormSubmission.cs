namespace WebFormManager.Domain.Entities;

public class FormSubmission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FormName { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}