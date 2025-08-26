using SFA.DAS.LearnerData.Messages;

namespace SFA.DAS.LearnerData.Services;

public class NServiceBusEventPublisher(IMessageSession messageSession) : IEventPublisher
{
    public async Task PublishLearnerDataUpdatedEventAsync(LearnerDataUpdatedEvent @event)
    {
        await messageSession.Publish(@event);
    }
} 