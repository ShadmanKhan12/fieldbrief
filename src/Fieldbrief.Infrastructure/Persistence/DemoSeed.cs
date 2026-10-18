using Fieldbrief.Domain.Engagements;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fieldbrief.Infrastructure.Persistence;

public static class DemoSeed
{
    public static readonly Guid DemoEngagementId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static async Task EnsureSeededAsync(IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FieldbriefDbContext>();
        var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DemoSeed");

        await db.Database.MigrateAsync(ct);

        if (await db.Engagements.AnyAsync(ct))
        {
            return;
        }

        var engagement = new Engagement
        {
            Id = DemoEngagementId,
            Code = "NORTHSTAR-01",
            CustomerName = "Northstar Logistics",
            Title = "Control-tower visibility for delayed outbound shipments",
            ProblemStatement =
                "Ops leads cannot see which delayed parcels will miss SLA until carriers email spreadsheets at end of day. " +
                "Three systems hold partial truth: WMS, carrier API, and the customer notification service.",
            DesiredOutcome =
                "A live exception queue with root-cause tags, SLA risk score, and a repeatable health check for each dependency " +
                "so the FDE team can leave a maintainable operating picture—not a one-off dashboard.",
            Status = EngagementStatus.Build,
            Owner = "A. Rahman",
            StartedAt = DateTimeOffset.UtcNow.AddDays(-18),
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var wms = new CustomerSystem
        {
            Name = "Warehouse Management API",
            Kind = "api",
            BaseUrl = "https://httpbin.org",
            IsCritical = true,
            Notes = "Source of pick/pack events. Demo checks hit httpbin."
        };
        wms.HealthChecks.Add(new HealthCheckDefinition
        {
            Name = "WMS health",
            Method = "GET",
            Path = "/status/200",
            ExpectedStatusCode = 200
        });

        var carrier = new CustomerSystem
        {
            Name = "Carrier tracking API",
            Kind = "api",
            BaseUrl = "https://httpbin.org",
            IsCritical = true,
            Notes = "Rate-limited in prod; demo uses httpbin delay/status endpoints."
        };
        carrier.HealthChecks.Add(new HealthCheckDefinition
        {
            Name = "Carrier reachable",
            Method = "GET",
            Path = "/status/200",
            ExpectedStatusCode = 200
        });
        carrier.HealthChecks.Add(new HealthCheckDefinition
        {
            Name = "Carrier auth probe (expected fail in demo)",
            Method = "GET",
            Path = "/status/401",
            ExpectedStatusCode = 200
        });

        var notify = new CustomerSystem
        {
            Name = "Customer notification service",
            Kind = "api",
            BaseUrl = "https://httpbin.org",
            IsCritical = false,
            Notes = "SMS/email fan-out. Non-blocking for exception queue MVP."
        };
        notify.HealthChecks.Add(new HealthCheckDefinition
        {
            Name = "Notify health",
            Method = "GET",
            Path = "/status/200",
            ExpectedStatusCode = 200
        });

        engagement.Systems.Add(wms);
        engagement.Systems.Add(carrier);
        engagement.Systems.Add(notify);

        engagement.Decisions.Add(new FieldDecision
        {
            Title = "Use WMS timestamps as source of truth for SLA clock",
            Context = "Carrier scan times lag 15–40 minutes during peak; ops was double-counting delays.",
            Decision = "SLA risk is computed from WMS packed_at + lane SLA table. Carrier ETA is enrichment only.",
            Consequences = "Requires stable packed_at; if WMS backlog grows, risk scores go stale—monitor WMS health first.",
            MadeBy = "A. Rahman",
            FeedBackToProduct = true,
            MadeAt = DateTimeOffset.UtcNow.AddDays(-10)
        });
        engagement.Decisions.Add(new FieldDecision
        {
            Title = "Defer LLM summarization of exception notes to phase 2",
            Context = "Stakeholders asked for AI summaries in week 1.",
            Decision = "Ship deterministic exception queue first. Add optional local LLM summaries after pilot metrics exist.",
            Consequences = "Avoids model spend and review overhead during discovery; keeps AI optional.",
            MadeBy = "A. Rahman",
            FeedBackToProduct = true,
            MadeAt = DateTimeOffset.UtcNow.AddDays(-7)
        });

        var checklist = new (string phase, string title, bool done)[]
        {
            ("discovery", "Map systems and data owners", true),
            ("discovery", "Capture success metrics with ops lead", true),
            ("discovery", "Identify critical path dependencies", true),
            ("build", "Stand up exception queue schema", true),
            ("build", "Wire WMS + carrier health checks", true),
            ("build", "Define SLA risk rules with ops", false),
            ("pilot", "Shadow mode for one DC", false),
            ("pilot", "On-call runbook + rollback notes", false),
            ("production", "Handoff checklist signed by customer eng", false)
        };
        for (var i = 0; i < checklist.Length; i++)
        {
            engagement.Checklist.Add(new ChecklistItem
            {
                Phase = checklist[i].phase,
                Title = checklist[i].title,
                IsDone = checklist[i].done,
                SortOrder = i,
                CompletedAt = checklist[i].done ? DateTimeOffset.UtcNow.AddDays(-i) : null
            });
        }

        engagement.Notes.Add(new FieldNote
        {
            Author = "A. Rahman",
            Body = "DC-East ops lead cares about 'miss risk before 2pm' more than fancy charts. Keep the queue boring and fast.",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-3)
        });
        engagement.Notes.Add(new FieldNote
        {
            Author = "A. Rahman",
            Body = "Carrier 401 probe left failing on purpose in the demo seed so FDE candidates can show triage.",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        });

        db.Engagements.Add(engagement);
        await db.SaveChangesAsync(ct);
        log.LogInformation("Seeded demo engagement {Code}", engagement.Code);
    }
}
