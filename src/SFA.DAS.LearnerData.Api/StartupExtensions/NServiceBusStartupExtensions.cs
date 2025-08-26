using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.NewtonsoftJson;
using SFA.DAS.LearnerData.Configuration;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class NServiceBusStartupExtensions
{
    private const string EndpointName = "SFA.DAS.LearnerData.Api";
    private const string ErrorEndpointName = $"{EndpointName}-error";

    public static IEndpointInstance AddNServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        var endpointConfiguration = CreateEndpointConfiguration(configuration);
        var endpointInstance = NServiceBus.Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        
        services.AddSingleton<IMessageSession>(endpointInstance);
        
        return endpointInstance;
    }

    public static void StopNServiceBus(this IEndpointInstance? endpointInstance)
    {
        endpointInstance?.Stop().GetAwaiter().GetResult();
    }

    private static EndpointConfiguration CreateEndpointConfiguration(IConfiguration configuration)
    {
        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        
        ConfigureBasicSettings(endpointConfiguration);
        ConfigureSerialization(endpointConfiguration);
        ConfigureMessageConventions(endpointConfiguration);
        ConfigureTransport(endpointConfiguration, configuration);
        ConfigureLicense(endpointConfiguration, configuration);

        return endpointConfiguration;
    }

    private static void ConfigureBasicSettings(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo(ErrorEndpointName);
    }

    private static void ConfigureSerialization(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    }

    private static void ConfigureMessageConventions(EndpointConfiguration endpointConfiguration)
    {
        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(t => t.Namespace?.EndsWith(".Commands") == true);
        conventions.DefiningEventsAs(t => t.Namespace?.EndsWith(".Events") == true);
        conventions.DefiningMessagesAs(t => t.Namespace?.EndsWith(".Messages") == true);
    }

    private static void ConfigureTransport(EndpointConfiguration endpointConfiguration, IConfiguration configuration)
    {
        var learnerDataConfig = configuration.GetSection("LearnerDataApi").Get<LearnerDataApi>();
        var connectionString = learnerDataConfig?.NServiceBusConnectionString;

#if DEBUG
        var transport = endpointConfiguration.UseTransport<LearningTransport>();
        transport.StorageDirectory(GetLearningTransportStorageDirectory());
#else
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("NServiceBus connection string is required for production environment");
        }
        
        var azureTransport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        azureTransport.ConnectionString(connectionString);
        azureTransport.SubscriptionRuleNamingConvention(AzureRuleNameShortener.Shorten);
#endif
    }

    private static void ConfigureLicense(EndpointConfiguration endpointConfiguration, IConfiguration configuration)
    {
        var learnerDataConfig = configuration.GetSection("LearnerDataApi").Get<LearnerDataApi>();
        var license = learnerDataConfig?.NServiceBusLicense;
        
        if (!string.IsNullOrEmpty(license))
        {
            var decodedLicense = WebUtility.HtmlDecode(license);
            endpointConfiguration.License(decodedLicense);
        }
    }

    private static string GetLearningTransportStorageDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var srcIndex = currentDirectory.IndexOf("src");
        var baseDirectory = srcIndex >= 0 ? currentDirectory.Substring(0, srcIndex) : currentDirectory;
        return Path.Combine(baseDirectory, "src/.learningtransport");
    }
}

internal static class AzureRuleNameShortener
{
    private const int AzureServiceBusRuleNameMaxLength = 50;

    public static string Shorten(Type type)
    {
        var ruleName = type.FullName;
        if (ruleName!.Length <= AzureServiceBusRuleNameMaxLength)
        {
            return ruleName;
        }

        var bytes = System.Text.Encoding.Default.GetBytes(ruleName);
        var hash = System.Security.Cryptography.MD5.HashData(bytes);
        return new Guid(hash).ToString();
    }
} 