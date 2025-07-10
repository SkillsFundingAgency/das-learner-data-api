using SFA.DAS.LearnerData.Messages;

namespace SFA.DAS.LearnerData.Services;

public interface IEventPublisher
{
    Task PublishLearnerDataUpdatedEventAsync(LearnerDataUpdatedEvent @event);
} 