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

public class SaveLearnerCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_Save_When_Learner_Exists_And_No_Changes(
        SaveLearnerCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        var response = new SaveLearnerCommandResponse { Id = 1, Result = SaveLearnerResult.Updated };
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John", LastName = "Doe"};
        var changeSummary = new ChangeSummary { Changes = [] };
        
        repository
            .Setup(x => x.Save(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();
            
        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, command.StandardCode, command.AcademicYear, It.IsAny<CancellationToken>()))
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
    public async Task Handle_Save_When_Learner_Exists_And_String_Changes_Detected(
        SaveLearnerCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        command.FirstName = "Jane";
        var response = new SaveLearnerCommandResponse { Id = 1, Result = SaveLearnerResult.Updated };
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John", LastName = "Doe"};
        var changeSummary = new ChangeSummary 
        { 
            Changes = [new FirstNameChange { OldValue = "John", NewValue = "Jane" }]
        };
        
        repository
            .Setup(x => x.Save(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();
            
        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, command.StandardCode, command.AcademicYear, It.IsAny<CancellationToken>()))
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
                e.LearnerId == response.Id && 
                e.ChangeSummary.HasChanges)), Times.Once);
        
        repository.Verify();
        changeTrackingService.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Save_When_Learner_Exists_And_DateOfBirth_Changes_Detected(
        SaveLearnerCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        command.Dob = new DateTime(1995, 6, 15);
        var response = new SaveLearnerCommandResponse { Id = 1, Result = SaveLearnerResult.Updated };
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John", Dob = new DateTime(1990, 1, 1) };
        var changeSummary = new ChangeSummary 
        { 
            Changes = [new DobChange { OldValue = new DateTime(1990, 1, 1), NewValue = new DateTime(1995, 6, 15) }]
        };
        
        repository
            .Setup(x => x.Save(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();
            
        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, command.StandardCode, command.AcademicYear, It.IsAny<CancellationToken>()))
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
                e.LearnerId == response.Id && 
                e.ChangeSummary.HasChanges)), Times.Once);
        
        repository.Verify();
        changeTrackingService.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Save_When_Learner_Exists_And_Numeric_Changes_Detected(
        SaveLearnerCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        command.TrainingPrice = 15000;
        var response = new SaveLearnerCommandResponse { Id = 1, Result = SaveLearnerResult.Updated };
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John", TrainingPrice = 12000 };
        var changeSummary = new ChangeSummary 
        { 
            Changes = [
                new TrainingPriceChange { OldValue = 12000, NewValue = 15000 }
            ]
        };
        
        repository
            .Setup(x => x.Save(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();
            
        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, command.StandardCode, command.AcademicYear, It.IsAny<CancellationToken>()))
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
                e.LearnerId == response.Id && 
                e.ChangeSummary.HasChanges)), Times.Once);
        
        repository.Verify();
        changeTrackingService.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Save_When_Learner_Exists_And_Boolean_Changes_Detected(
        SaveLearnerCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        command.FirstName = "Jane";
        var response = new SaveLearnerCommandResponse { Id = 1, Result = SaveLearnerResult.Updated };
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John" };
        var changeSummary = new ChangeSummary 
        { 
            Changes = [new FirstNameChange { OldValue = "John", NewValue = "Jane" }]
        };
        
        repository
            .Setup(x => x.Save(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();
            
        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, command.StandardCode, command.AcademicYear, It.IsAny<CancellationToken>()))
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
                e.LearnerId == response.Id && 
                e.ChangeSummary.HasChanges)), Times.Once);
        
        repository.Verify();
        changeTrackingService.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Save_When_Learner_Exists_And_Nullable_Changes_Detected(
        SaveLearnerCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IChangeTrackingService> changeTrackingService,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        command.Email = "newemail@example.com";
        var response = new SaveLearnerCommandResponse { Id = 1, Result = SaveLearnerResult.Updated };
        var existingLearner = new Learner { Uln = command.Uln, FirstName = "John", Email = "oldemail@example.com" };
        var changeSummary = new ChangeSummary 
        { 
            Changes = [
                new EmailChange { OldValue = "oldemail@example.com", NewValue = "newemail@example.com" }
            ]
        };
        
        repository
            .Setup(x => x.Save(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();
            
        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, command.StandardCode, command.AcademicYear, It.IsAny<CancellationToken>()))
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
                e.LearnerId == response.Id && 
                e.ChangeSummary.HasChanges)), Times.Once);
        
        repository.Verify();
        changeTrackingService.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Save_When_Learner_Does_Not_Exist(
        SaveLearnerCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        [Frozen] Mock<IEventPublisher> eventPublisher,
        SaveLearnerCommandHandler sut)
    {
        // Arrange
        command.Uln = 100030001;
        var response = new SaveLearnerCommandResponse { Id = 1, Result = SaveLearnerResult.Created };
        
        repository
            .Setup(x => x.Save(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();
            
        repository
            .Setup(x => x.Get(command.Ukprn, command.Uln, command.StandardCode, command.AcademicYear, It.IsAny<CancellationToken>()))
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