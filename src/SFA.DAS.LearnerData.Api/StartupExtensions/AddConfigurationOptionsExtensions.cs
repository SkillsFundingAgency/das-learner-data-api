using Microsoft.Extensions.Options;
using SFA.DAS.LearnerData.Configuration;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class AddConfigurationOptionsExtension
{
    public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<LearnerDataApi>(configuration.GetSection("LearnerDataApi"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<LearnerDataApi>>().Value);
    }
}