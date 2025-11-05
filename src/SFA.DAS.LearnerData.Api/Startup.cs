using System.Net;
using Asp.Versioning;
using FluentValidation;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.LearnerData.Api.HttpResponseExtensions;
using SFA.DAS.LearnerData.Api.Middleware;
using SFA.DAS.LearnerData.Api.Models;
using SFA.DAS.LearnerData.Api.StartupExtensions;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.LearnerData.Configuration;

namespace SFA.DAS.LearnerData.Api;

public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
    private readonly IHostEnvironment _environment = environment;
    private readonly IConfiguration _configuration = configuration.BuildDasConfiguration();
    private IEndpointInstance? _endpointInstance;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
        });

        services.AddConfigurationOptions(_configuration);
        services.AddSingleton(_configuration);
        services.AddApplicationServices();
        services.AddHttpContextAccessor();

        if (!_environment.IsDevelopment())
        {
            var azureAdConfiguration = _configuration
                .GetSection("AzureAd")
                .Get<AzureActiveDirectoryConfiguration>();

            var policies = new Dictionary<string, string>
            {
                { PolicyNames.Default, RoleNames.Default }
            };

            services.AddAuthentication(azureAdConfiguration, policies);
        }

        services.AddMvc(mvcOptions =>
            {
                if (!_environment.IsDevelopment())
                {
                    mvcOptions.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                }

                mvcOptions.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
            })
            .AddNewtonsoftJson();

        services.AddControllers();

        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<SaveLearnerNewCommand>());

        var config = _configuration.GetSection<LearnerDataApi>();

        services.AddDasHealthChecks();

        services.AddEntityFrameworkForLearnerData(config);

        services.AddDasDataProtection(config, _environment)
            .AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "LearnerDataApi", Version = "v1" });
                options.OperationFilter<SwaggerVersionHeaderFilter>();
            })
            .AddSwaggerGenNewtonsoftSupport();

        services.AddApiVersioning(opt => { opt.ApiVersionReader = new HeaderApiVersionReader("X-Version"); });

        services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { EnableAdaptiveSampling = false });

        _endpointInstance = services.AddNServiceBus(_configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseDasHealthChecks();
        app.UseMiddleware<SecurityHeadersMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        
        app.Use(async (context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                if (context.Response.Headers.ContainsKey("X-Powered-By"))
                {
                    context.Response.Headers.Remove("X-Powered-By");
                }

                return Task.CompletedTask;
            });

            await next();
        });

        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                if (exception is ValidationException validationException)
                {
                    var errorResponse = new FluentValidationErrorResponse
                    {
                        Errors = validationException.Errors
                    };

                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteJsonAsync(errorResponse);
                }
            });
        });

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "LearnerData v1");
            options.RoutePrefix = string.Empty;
        });

        appLifetime.ApplicationStopping.Register(() => _endpointInstance?.StopNServiceBus());
    }
}