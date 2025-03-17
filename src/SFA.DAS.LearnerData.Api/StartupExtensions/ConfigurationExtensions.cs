using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class ConfigurationExtensions
{
    public static T GetSection<T>(this IConfiguration configuration)
    {
        return configuration
            .GetSection(typeof(T).Name)
            .Get<T>();
    }

    public static IConfiguration BuildDasConfiguration(this IConfiguration configuration)
    {
        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
        if (configuration["EnvironmentName"].Equals("Development", StringComparison.CurrentCultureIgnoreCase))
        {
            config.AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true);
        }
#endif
        config.AddEnvironmentVariables();

        config.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            }
        );

        return config.Build();
    }
}