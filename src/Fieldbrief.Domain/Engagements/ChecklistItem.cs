namespace Fieldbrief.Domain.Engagements;

public class ChecklistItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Phase { get; set; } = "discovery"; // discovery | build | pilot | production
    public bool IsDone { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
