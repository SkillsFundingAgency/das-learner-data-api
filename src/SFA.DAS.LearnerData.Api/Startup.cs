using System.Net;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.LearnerData.Api.HttpResponseExtensions;
using SFA.DAS.LearnerData.Api.Middleware;
using SFA.DAS.LearnerData.Api.Models;
using SFA.DAS.LearnerData.Api.StartupExtensions;
using SFA.DAS.LearnerData.Application.Commands;
using SFA.DAS.LearnerData.Configuration;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.Microsoft;

namespace SFA.DAS.LearnerData.Api;

public class Startup
{
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _environment = environment;
        _configuration = configuration.BuildDasConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
        });

        services.AddConfigurationOptions(_configuration);
        services.AddSingleton(_configuration);

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

        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<Startup>()
            .AddValidatorsFromAssemblyContaining<CreateLearnerCommandValidator>();

        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<CreateLearnerCommand>());

        var config = _configuration.GetSection<LearnerDataApi>();

        services.AddDasHealthChecks(config);

        services.AddEntityFrameworkForLearnerData(config)
            .AddEntityFrameworkUnitOfWork<LearnerDataDbContext>()
            .AddNServiceBusClientUnitOfWork();

        services.AddDasDataProtection(config, _environment)
            .AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "LearnerDataApi", Version = "v1" });
                options.OperationFilter<SwaggerVersionHeaderFilter>();
            })
            .AddSwaggerGenNewtonsoftSupport();

        services.AddApiVersioning(opt => { opt.ApiVersionReader = new HeaderApiVersionReader("X-Version"); });

        services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { EnableAdaptiveSampling = false });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<SecurityHeadersMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseDasHealthChecks();

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
                var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
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
    }

    public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
    {
        var config = _configuration.GetSection<LearnerDataApi>();
        serviceProvider.StartNServiceBus(config);

        // Replacing ClientOutboxPersisterV2 with a local version to fix unit of work issue due to propagating Task up the chain rather than awaiting on DB Command.
        // not clear why this fixes the issue. Attempted to make the change in SFA.DAS.Nservicebus.SqlServer however it conflicts when upgraded with SFA.DAS.UnitOfWork.Nservicebus
        // which would require upgrading to NET6 to resolve.
        var serviceDescriptor = serviceProvider.FirstOrDefault(serv => serv.ServiceType == typeof(IClientOutboxStorageV2));
        serviceProvider.Remove(serviceDescriptor);
        serviceProvider.AddScoped<IClientOutboxStorageV2, ClientOutboxPersisterV2>();
    }
}