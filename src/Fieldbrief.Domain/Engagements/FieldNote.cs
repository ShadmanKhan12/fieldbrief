namespace Fieldbrief.Domain.Engagements;

public class FieldNote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;

    public string Author { get; set; } = "FDE";
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
