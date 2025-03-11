namespace WebFormManager.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Data { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}