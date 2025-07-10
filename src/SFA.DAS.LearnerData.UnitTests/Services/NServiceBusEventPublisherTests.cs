using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NServiceBus;
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

        messageSession.Verify(x => x.Send(@event, It.IsAny<SendOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }
} 