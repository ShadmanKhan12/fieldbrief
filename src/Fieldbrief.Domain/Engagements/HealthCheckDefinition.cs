namespace Fieldbrief.Domain.Engagements;

public class HealthCheckDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerSystemId { get; set; }
    public CustomerSystem CustomerSystem { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public string Path { get; set; } = "/health";
    public int ExpectedStatusCode { get; set; } = 200;
    public int TimeoutMs { get; set; } = 5000;

    public ICollection<HealthCheckResult> Results { get; set; } = new List<HealthCheckResult>();
}

public class HealthCheckResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HealthCheckDefinitionId { get; set; }
    public HealthCheckDefinition HealthCheckDefinition { get; set; } = null!;

    public DateTimeOffset RanAt { get; set; } = DateTimeOffset.UtcNow;
    public bool Passed { get; set; }
    public int? StatusCode { get; set; }
    public long DurationMs { get; set; }
    public string? Error { get; set; }
}
