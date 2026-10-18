using System.Diagnostics;
using Fieldbrief.Domain.Checks;
using Fieldbrief.Domain.Engagements;
using Fieldbrief.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fieldbrief.Infrastructure.Checks;

public class HttpHealthCheckRunner(FieldbriefDbContext db, IHttpClientFactory httpClientFactory)
{
    public async Task<HealthCheckResult> RunAsync(Guid checkId, CancellationToken ct = default)
    {
        var check = await db.HealthChecks
            .Include(c => c.CustomerSystem)
            .FirstOrDefaultAsync(c => c.Id == checkId, ct)
            ?? throw new InvalidOperationException("Health check not found.");

        var client = httpClientFactory.CreateClient("health");
        client.Timeout = TimeSpan.FromMilliseconds(Math.Clamp(check.TimeoutMs, 500, 30000));

        var url = Combine(check.CustomerSystem.BaseUrl, check.Path);
        var sw = Stopwatch.StartNew();
        int? status = null;
        string? error = null;

        try
        {
            using var request = new HttpRequestMessage(new HttpMethod(check.Method), url);
            using var response = await client.SendAsync(request, ct);
            status = (int)response.StatusCode;
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        finally
        {
            sw.Stop();
        }

        var result = new HealthCheckResult
        {
            HealthCheckDefinitionId = check.Id,
            RanAt = DateTimeOffset.UtcNow,
            StatusCode = status,
            DurationMs = sw.ElapsedMilliseconds,
            Error = error,
            Passed = HealthCheckEvaluator.Passed(status, check.ExpectedStatusCode, error)
        };

        db.HealthCheckResults.Add(result);
        await db.SaveChangesAsync(ct);
        return result;
    }

    public async Task<IReadOnlyList<HealthCheckResult>> RunForEngagementAsync(Guid engagementId, CancellationToken ct = default)
    {
        var checkIds = await db.HealthChecks
            .Where(c => c.CustomerSystem.EngagementId == engagementId)
            .Select(c => c.Id)
            .ToListAsync(ct);

        var results = new List<HealthCheckResult>();
        foreach (var id in checkIds)
        {
            results.Add(await RunAsync(id, ct));
        }

        return results;
    }

    private static string Combine(string baseUrl, string path)
    {
        return $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
    }
}
