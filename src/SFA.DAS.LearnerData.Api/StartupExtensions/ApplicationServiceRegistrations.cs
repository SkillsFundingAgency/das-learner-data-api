using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Services;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ILearnerRepository, LearnerRepository>();
        services.AddTransient<IPagedLinkHeaderService, PagedLinkHeaderService>();
        services.AddTransient<IChangeTrackingService, ChangeTrackingService>();
        services.AddTransient<IEventPublisher, NServiceBusEventPublisher>();

        return services;
    }
}