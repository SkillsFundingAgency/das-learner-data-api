using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Messages;
using SFA.DAS.LearnerData.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Commands;

public class SaveLearnerNewCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_Save_When_Unapproved_Learner_Exists_And_No_Changes(
        SaveLearnerNewCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerNewCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        var response = new SaveLearnerNewCommandResponse { Id = 1, Result = SaveLearnerNewResult.Updated };
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John", LastName = "Doe" };
        var changeSummary = new ChangeSummary { Changes = [], HasMaterialChanges = false };

        repository
            .Setup(x => x.UpdateLearner(existingLearner, command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();

        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLearner)
            .Verifiable();

        changeTrackingService
            .Setup(x => x.DetectChanges(existingLearner, It.Is<Learner>(l => l.Uln == existingLearner.Uln)))
            .Returns(changeSummary)
            .Verifiable();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        result.Result.Should().Be(response.Result);

        eventPublisher.Verify(x => x.PublishLearnerDataUpdatedEventAsync(It.IsAny<LearnerDataUpdatedEvent>()), Times.Never);

        repository.Verify();
        changeTrackingService.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Save_When_Approved_Learner_Exists_And_NoMaterial_Changes(
        SaveLearnerNewCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerNewCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John", LastName = "Doe", ApprenticeshipId = 12345 };
        var changeSummary = new ChangeSummary { Changes = [], HasMaterialChanges = false };

        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLearner)
            .Verifiable();

        changeTrackingService
            .Setup(x => x.DetectChanges(existingLearner, It.Is<Learner>(l => l.Uln == existingLearner.Uln)))
            .Returns(changeSummary)
            .Verifiable();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingLearner.Id);
        result.Result.Should().Be(SaveLearnerNewResult.NotNeeded);

        eventPublisher.Verify(x => x.PublishLearnerDataUpdatedEventAsync(It.IsAny<LearnerDataUpdatedEvent>()), Times.Never);

        repository.Verify();
        changeTrackingService.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Save_When_Approved_Learner_Exists_And_There_Are_Material_Changes(
        SaveLearnerNewCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerNewCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        var response = new SaveLearnerNewCommandResponse { Id = 1, Result = SaveLearnerNewResult.Created };
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John", LastName = "Doe", ApprenticeshipId = 12345 };
        var changeSummary = new ChangeSummary { Changes = [], HasMaterialChanges = true };

        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLearner)
            .Verifiable();

        repository
            .Setup(x => x.AddLearner(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response)
            .Verifiable();

        changeTrackingService
            .Setup(x => x.DetectChanges(existingLearner, It.Is<Learner>(l => l.Uln == existingLearner.Uln)))
            .Returns(changeSummary)
            .Verifiable();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        result.Result.Should().Be(SaveLearnerNewResult.Created);

        eventPublisher.Verify(x => x.PublishLearnerDataUpdatedEventAsync(It.IsAny<LearnerDataUpdatedEvent>()), Times.Never);

        repository.Verify();
        changeTrackingService.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Save_When_Unapproved_Learner_Exists_And_Learner_Changes_Detected(
        SaveLearnerNewCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerNewCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        command.FirstName = "Jane";
        var response = new SaveLearnerNewCommandResponse { Id = 1, Result = SaveLearnerNewResult.Updated };
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John", LastName = "Doe" };
        var changeSummary = new ChangeSummary
        {
            Changes = [new FirstNameChange { OldValue = "John", NewValue = "Jane" }],
            HasMaterialChanges = true
        };

        repository
            .Setup(x => x.UpdateLearner(existingLearner, command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();

        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLearner)
            .Verifiable();

        changeTrackingService
            .Setup(x => x.DetectChanges(existingLearner, It.Is<Learner>(l => l.Uln == existingLearner.Uln)))
            .Returns(changeSummary)
            .Verifiable();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        result.Result.Should().Be(response.Result);

        eventPublisher.Verify(x => x.PublishLearnerDataUpdatedEventAsync(
            It.Is<LearnerDataUpdatedEvent>(e =>
                e.LearnerId == response.Id)), Times.Once);

        repository.Verify();
        changeTrackingService.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Save_When_Learner_Does_Not_Exist(
        SaveLearnerNewCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerNewCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        var response = new SaveLearnerNewCommandResponse { Id = 1, Result = SaveLearnerNewResult.Created };

        repository
            .Setup(x => x.AddLearner(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();

        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Learner?)null)
            .Verifiable();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        result.Result.Should().Be(response.Result);

        eventPublisher.Verify(x => x.PublishLearnerDataUpdatedEventAsync(It.IsAny<LearnerDataUpdatedEvent>()), Times.Never);

        repository.Verify();
    }
}