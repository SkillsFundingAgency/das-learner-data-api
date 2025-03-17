
using NServiceBus;

namespace SFA.DAS.LearnerData.Extensions;

public static class RoutingSettingsExtensions
{
    private const string LearnerDataMatchingMessageHandler = "SFA.DAS.LearnerData.MessageHandlers";

    public static void AddRouting(this RoutingSettings routingSettings)
    {
        //routingSettings.RouteToEndpoint(typeof(RunHealthCheckCommand), LearnerDataHandler);
    }
}