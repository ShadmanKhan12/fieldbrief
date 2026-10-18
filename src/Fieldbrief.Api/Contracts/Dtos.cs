namespace Fieldbrief.Api.Contracts;

public record EngagementListItemDto(
    Guid Id,
    string Code,
    string CustomerName,
    string Title,
    string Status,
    string Owner,
    DateTimeOffset UpdatedAt,
    int SystemCount,
    int OpenChecklistCount);

public record EngagementDetailDto(
    Guid Id,
    string Code,
    string CustomerName,
    string Title,
    string ProblemStatement,
    string DesiredOutcome,
    string Status,
    string Owner,
    DateTimeOffset StartedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<SystemDto> Systems,
    IReadOnlyList<DecisionDto> Decisions,
    IReadOnlyList<ChecklistItemDto> Checklist,
    IReadOnlyList<NoteDto> Notes);

public record SystemDto(
    Guid Id,
    string Name,
    string Kind,
    string BaseUrl,
    bool IsCritical,
    string? Notes,
    IReadOnlyList<HealthCheckDto> HealthChecks);

public record HealthCheckDto(
    Guid Id,
    string Name,
    string Method,
    string Path,
    int ExpectedStatusCode,
    HealthCheckResultDto? LastResult);

public record HealthCheckResultDto(
    Guid Id,
    DateTimeOffset RanAt,
    bool Passed,
    int? StatusCode,
    long DurationMs,
    string? Error);

public record DecisionDto(
    Guid Id,
    string Title,
    string Context,
    string Decision,
    string Consequences,
    string MadeBy,
    DateTimeOffset MadeAt,
    bool FeedBackToProduct);

public record ChecklistItemDto(
    Guid Id,
    string Title,
    string Phase,
    bool IsDone,
    int SortOrder,
    DateTimeOffset? CompletedAt);

public record NoteDto(Guid Id, string Author, string Body, DateTimeOffset CreatedAt);

public record CreateDecisionRequest(
    string Title,
    string Context,
    string Decision,
    string Consequences,
    bool FeedBackToProduct);

public record CreateNoteRequest(string Body, string? Author);

public record ToggleChecklistRequest(bool IsDone);
