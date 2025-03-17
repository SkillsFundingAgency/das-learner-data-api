using Microsoft.AspNetCore.DataProtection;
using SFA.DAS.LearnerData.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class DataProtectionStartupExtensions
{
    public static IServiceCollection AddDasDataProtection(this IServiceCollection services, LearnerDataApi configuration, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            return services;
        }
        
        var redisConnectionString = configuration.RedisConnectionString;
        var dataProtectionKeysDatabase = configuration.DataProtectionKeysDatabase;

        var redis = ConnectionMultiplexer.Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

        services.AddDataProtection()
            .SetApplicationName("das-learner-data-api")
            .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

        return services;
    }
}