using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SFA.DAS.LearnerData.Api.HealthChecks;
using SFA.DAS.LearnerData.Api.HttpResponseExtensions;
using SFA.DAS.LearnerData.Configuration;
using SFA.DAS.LearnerData.Data;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class HealthCheckStartupExtensions
{
    public static IServiceCollection AddDasHealthChecks(this IServiceCollection services, LearnerDataApi config)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<LearnerDataDbContext>("Sql Health Check")
            .AddCheck<NServiceBusHealthCheck>("NService Bus health check");

        return services;
    }

    public static IApplicationBuilder UseDasHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/ping", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = (context, _) =>
            {
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("");
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