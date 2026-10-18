using Fieldbrief.Api.Endpoints;
using Fieldbrief.Infrastructure;
using Fieldbrief.Infrastructure.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((ctx, services, cfg) => cfg
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    builder.Services.AddFieldbriefInfrastructure(builder.Configuration);
    builder.Services.AddOpenApi();
    builder.Services.AddCors(o => o.AddPolicy("dev", p =>
        p.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()));

    var app = builder.Build();
    app.UseSerilogRequestLogging();
    app.UseCors("dev");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.MapGet("/health", async (FieldbriefDbContext db, CancellationToken ct) =>
        await db.Database.CanConnectAsync(ct)
            ? Results.Ok(new { status = "Healthy" })
            : Results.Json(new { status = "Unhealthy" }, statusCode: 503));

    app.MapEngagementEndpoints();
    await DemoSeed.EnsureSeededAsync(app.Services);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
