using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.LearnerData.Configuration;
using SFA.DAS.LearnerData.Extensions;
using SFA.DAS.LearnerData.Infrastructure.ConnectionFactory;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using Endpoint = NServiceBus.Endpoint;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class NServiceBusStartup
{
    private const string EndpointName = "SFA.DAS.LearnerData.MessageHandlers";

    public static void StartNServiceBus(this UpdateableServiceProvider serviceProvider, LearnerDataApi configuration)
    {
        var connectionFactory = serviceProvider.GetService<IConnectionFactory>();

        var endpointConfiguration = new EndpointConfiguration(EndpointName)
            .UseErrorQueue($"{EndpointName}-errors")
            .UseMessageConventions()
            .UseNewtonsoftJsonSerializer()
            .UseOutbox(true)
            .UseServicesBuilder(serviceProvider)
            .UseSqlServerPersistence(() => connectionFactory.CreateConnection(configuration.DatabaseConnectionString))
            .UseUnitOfWork();

        if (configuration.NServiceBusConnectionString.Equals("UseDevelopmentStorage=true", StringComparison.CurrentCultureIgnoreCase))
        {
            endpointConfiguration.UseLearningTransport(s => s.AddRouting());
        }
        else
        {
            endpointConfiguration.UseAzureServiceBusTransport(configuration.NServiceBusConnectionString, r => { r.AddRouting(); });
        }

        if (!string.IsNullOrEmpty(configuration.NServiceBusLicense))
        {
            endpointConfiguration.License(configuration.NServiceBusLicense);
        }

        var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

        serviceProvider.AddSingleton(p => endpoint)
            .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();
    }
}