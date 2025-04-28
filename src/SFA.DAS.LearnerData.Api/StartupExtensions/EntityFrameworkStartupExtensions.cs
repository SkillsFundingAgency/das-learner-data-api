using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerData.Configuration;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.LearnerData.Infrastructure;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class EntityFrameworkStartupExtensions
{
    public static IServiceCollection AddEntityFrameworkForLearnerData(this IServiceCollection services, LearnerDataApi config)
    {
        services.AddDbContext<LearnerDataDbContext>((provider, options) =>
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(config.DatabaseConnectionString);
            var useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);
            
            var sqlConnection = new SqlConnection(config.DatabaseConnectionString);
            
            if (useManagedIdentity)
            {
                sqlConnection.AccessToken = SqlTokenGenerator.Generate();
            }

            options.UseSqlServer(sqlConnection);
        });

        return services;
    }
}