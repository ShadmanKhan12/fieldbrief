namespace Fieldbrief.Domain.Engagements;

public class Engagement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ProblemStatement { get; set; } = string.Empty;
    public string DesiredOutcome { get; set; } = string.Empty;
    public EngagementStatus Status { get; set; } = EngagementStatus.Discovery;
    public string Owner { get; set; } = "FDE";
    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<CustomerSystem> Systems { get; set; } = new List<CustomerSystem>();
    public ICollection<FieldDecision> Decisions { get; set; } = new List<FieldDecision>();
    public ICollection<ChecklistItem> Checklist { get; set; } = new List<ChecklistItem>();
    public ICollection<FieldNote> Notes { get; set; } = new List<FieldNote>();
}

public enum EngagementStatus
{
    Discovery = 0,
    Build = 1,
    Pilot = 2,
    Production = 3,
    Closed = 4
}
