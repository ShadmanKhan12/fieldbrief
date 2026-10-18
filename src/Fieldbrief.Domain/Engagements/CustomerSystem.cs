namespace Fieldbrief.Domain.Engagements;

public class CustomerSystem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Kind { get; set; } = "api"; // api | db | queue | file | ui
    public string BaseUrl { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsCritical { get; set; }

    public ICollection<HealthCheckDefinition> HealthChecks { get; set; } = new List<HealthCheckDefinition>();
}
