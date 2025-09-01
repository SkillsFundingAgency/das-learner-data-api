using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Messages;
using SFA.DAS.LearnerData.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Services;

public class NServiceBusEventPublisherTests
{
    [Test, MoqAutoData]
    public async Task PublishLearnerDataUpdatedEventAsync_Should_Publish_Event(
        LearnerDataUpdatedEvent @event,
        [Frozen] Mock<IMessageSession> messageSession,
        NServiceBusEventPublisher sut
    )
    {
        await sut.PublishLearnerDataUpdatedEventAsync(@event);

        messageSession.Verify(x => x.Publish(@event, It.IsAny<PublishOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }
} 