using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SFA.DAS.LearnerData.Api.HttpResponseExtensions;
using SFA.DAS.LearnerData.Data;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class HealthCheckStartupExtensions
{
    public static IServiceCollection AddDasHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<LearnerDataDbContext>("Sql Health Check");

        return services;
    }

    public static IApplicationBuilder UseDasHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/info", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = (context, _) =>
            {
                context.Response.ContentType = "application/json";
                var info = new
                {
                    Version = "1.0.0",
                    Name = "Learner Data Inner API"
                };
                return context.Response.WriteAsync(JsonSerializer.Serialize(info));
            }
        });

        return app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = (httpContext, report) => httpContext.Response.WriteJsonAsync(new
            {
                report.Status,
                report.TotalDuration,
                Results = report.Entries.ToDictionary(
                    e => e.Key,
                    e => new
                    {
                        e.Value.Status,
                        e.Value.Duration,
                        e.Value.Description,
                        e.Value.Data
                    })
            })
        });
    }
}