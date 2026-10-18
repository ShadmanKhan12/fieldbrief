using Fieldbrief.Api.Contracts;
using Fieldbrief.Domain.Engagements;
using Fieldbrief.Infrastructure.Checks;
using Fieldbrief.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fieldbrief.Api.Endpoints;

public static class EngagementEndpoints
{
    public static IEndpointRouteBuilder MapEngagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/engagements").WithTags("Engagements");

        group.MapGet("/", async (FieldbriefDbContext db, CancellationToken ct) =>
        {
            var items = await db.Engagements.AsNoTracking()
                .OrderByDescending(e => e.UpdatedAt)
                .Select(e => new EngagementListItemDto(
                    e.Id,
                    e.Code,
                    e.CustomerName,
                    e.Title,
                    e.Status.ToString(),
                    e.Owner,
                    e.UpdatedAt,
                    e.Systems.Count,
                    e.Checklist.Count(c => !c.IsDone)))
                .ToListAsync(ct);
            return Results.Ok(items);
        });

        group.MapGet("/{id:guid}", async (Guid id, FieldbriefDbContext db, CancellationToken ct) =>
        {
            var e = await db.Engagements.AsNoTracking()
                .Include(x => x.Systems)
                    .ThenInclude(s => s.HealthChecks)
                        .ThenInclude(h => h.Results)
                .Include(x => x.Decisions)
                .Include(x => x.Checklist)
                .Include(x => x.Notes)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (e is null) return Results.NotFound();
            return Results.Ok(MapDetail(e));
        });

        group.MapPost("/{id:guid}/decisions", async (
            Guid id,
            CreateDecisionRequest req,
            FieldbriefDbContext db,
            CancellationToken ct) =>
        {
            if (!await db.Engagements.AnyAsync(e => e.Id == id, ct)) return Results.NotFound();
            if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Decision))
            {
                return Results.BadRequest(new { title = "Title and decision are required." });
            }

            var decision = new FieldDecision
            {
                EngagementId = id,
                Title = req.Title.Trim(),
                Context = req.Context?.Trim() ?? "",
                Decision = req.Decision.Trim(),
                Consequences = req.Consequences?.Trim() ?? "",
                FeedBackToProduct = req.FeedBackToProduct,
                MadeAt = DateTimeOffset.UtcNow
            };
            db.Decisions.Add(decision);
            var eng = await db.Engagements.FirstAsync(e => e.Id == id, ct);
            eng.UpdatedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/v1/engagements/{id}", new DecisionDto(
                decision.Id, decision.Title, decision.Context, decision.Decision,
                decision.Consequences, decision.MadeBy, decision.MadeAt, decision.FeedBackToProduct));
        });

        group.MapPost("/{id:guid}/notes", async (
            Guid id,
            CreateNoteRequest req,
            FieldbriefDbContext db,
            CancellationToken ct) =>
        {
            if (!await db.Engagements.AnyAsync(e => e.Id == id, ct)) return Results.NotFound();
            if (string.IsNullOrWhiteSpace(req.Body)) return Results.BadRequest(new { title = "Body is required." });

            var note = new FieldNote
            {
                EngagementId = id,
                Body = req.Body.Trim(),
                Author = string.IsNullOrWhiteSpace(req.Author) ? "FDE" : req.Author.Trim()
            };
            db.Notes.Add(note);
            var eng = await db.Engagements.FirstAsync(e => e.Id == id, ct);
            eng.UpdatedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/v1/engagements/{id}", new NoteDto(note.Id, note.Author, note.Body, note.CreatedAt));
        });

        group.MapPatch("/{engagementId:guid}/checklist/{itemId:guid}", async (
            Guid engagementId,
            Guid itemId,
            ToggleChecklistRequest req,
            FieldbriefDbContext db,
            CancellationToken ct) =>
        {
            var item = await db.ChecklistItems.FirstOrDefaultAsync(
                c => c.Id == itemId && c.EngagementId == engagementId, ct);
            if (item is null) return Results.NotFound();

            item.IsDone = req.IsDone;
            item.CompletedAt = req.IsDone ? DateTimeOffset.UtcNow : null;
            var eng = await db.Engagements.FirstAsync(e => e.Id == engagementId, ct);
            eng.UpdatedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/checks/run", async (
            Guid id,
            HttpHealthCheckRunner runner,
            FieldbriefDbContext db,
            CancellationToken ct) =>
        {
            if (!await db.Engagements.AnyAsync(e => e.Id == id, ct)) return Results.NotFound();
            var results = await runner.RunForEngagementAsync(id, ct);
            var dto = results.Select(r => new HealthCheckResultDto(
                r.Id, r.RanAt, r.Passed, r.StatusCode, r.DurationMs, r.Error)).ToList();
            return Results.Ok(dto);
        });

        group.MapPost("/checks/{checkId:guid}/run", async (
            Guid checkId,
            HttpHealthCheckRunner runner,
            CancellationToken ct) =>
        {
            try
            {
                var r = await runner.RunAsync(checkId, ct);
                return Results.Ok(new HealthCheckResultDto(
                    r.Id, r.RanAt, r.Passed, r.StatusCode, r.DurationMs, r.Error));
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
        });

        return app;
    }

    private static EngagementDetailDto MapDetail(Engagement e) => new(
        e.Id,
        e.Code,
        e.CustomerName,
        e.Title,
        e.ProblemStatement,
        e.DesiredOutcome,
        e.Status.ToString(),
        e.Owner,
        e.StartedAt,
        e.UpdatedAt,
        e.Systems.Select(s => new SystemDto(
            s.Id,
            s.Name,
            s.Kind,
            s.BaseUrl,
            s.IsCritical,
            s.Notes,
            s.HealthChecks.Select(h => new HealthCheckDto(
                h.Id,
                h.Name,
                h.Method,
                h.Path,
                h.ExpectedStatusCode,
                h.Results.OrderByDescending(r => r.RanAt).Select(r => new HealthCheckResultDto(
                    r.Id, r.RanAt, r.Passed, r.StatusCode, r.DurationMs, r.Error)).FirstOrDefault()
            )).ToList()
        )).ToList(),
        e.Decisions.OrderByDescending(d => d.MadeAt).Select(d => new DecisionDto(
            d.Id, d.Title, d.Context, d.Decision, d.Consequences, d.MadeBy, d.MadeAt, d.FeedBackToProduct)).ToList(),
        e.Checklist.OrderBy(c => c.SortOrder).Select(c => new ChecklistItemDto(
            c.Id, c.Title, c.Phase, c.IsDone, c.SortOrder, c.CompletedAt)).ToList(),
        e.Notes.OrderByDescending(n => n.CreatedAt).Select(n => new NoteDto(n.Id, n.Author, n.Body, n.CreatedAt)).ToList());
}
