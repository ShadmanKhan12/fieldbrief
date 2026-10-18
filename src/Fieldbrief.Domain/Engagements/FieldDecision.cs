namespace Fieldbrief.Domain.Engagements;

public class FieldDecision
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string Consequences { get; set; } = string.Empty;
    public string MadeBy { get; set; } = "FDE";
    public DateTimeOffset MadeAt { get; set; } = DateTimeOffset.UtcNow;
    public bool FeedBackToProduct { get; set; }
}
