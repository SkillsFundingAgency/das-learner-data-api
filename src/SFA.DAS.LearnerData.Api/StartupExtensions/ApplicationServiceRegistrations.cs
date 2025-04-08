using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ILearnerDataRepository, LearnerDataRepository>();

        return services;
    }
}