using Fieldbrief.Infrastructure.Checks;
using Fieldbrief.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fieldbrief.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFieldbriefInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is required.");

        services.AddDbContext<FieldbriefDbContext>(o => o.UseNpgsql(cs));
        services.AddHttpClient("health");
        services.AddScoped<HttpHealthCheckRunner>();
        return services;
    }
}
