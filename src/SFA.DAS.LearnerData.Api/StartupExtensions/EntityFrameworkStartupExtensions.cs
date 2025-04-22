using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerData.Configuration;
using SFA.DAS.LearnerData.Data;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class EntityFrameworkStartupExtensions
{
    public static IServiceCollection AddEntityFrameworkForLearnerData(this IServiceCollection services, LearnerDataApi config)
    {
        services.AddDbContext<LearnerDataDbContext>((provider, options) => options.UseSqlServer(new SqlConnection(config.DatabaseConnectionString)));

        return services;
    }
}