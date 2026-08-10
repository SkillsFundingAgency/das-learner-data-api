using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Messages;
using SFA.DAS.LearnerData.Services;

namespace SFA.DAS.LearnerData.UnitTests.Services;

[TestFixture]
public class ChangeTrackingServiceTests
{
    [Test, AutoData]
    public void DetectChanges_WhenNoChanges_ShouldReturnEmptyChanges(ChangeTrackingService service)
    {
        // Arrange
        var existingLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF123"
        };

        var newLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = existingLearner.ReceivedDate,
            CorrelationId = existingLearner.CorrelationId,
            ConsumerReference = "REF123"
        };

        // Act
        var result = service.DetectChanges(existingLearner, newLearner);

        // Assert
        result.Should().NotBeNull();
        result.HasLearnerChanges.Should().BeFalse();
        result.Changes.Should().BeEmpty();
    }

    [Test, AutoData]
    public void DetectChanges_WhenAllFieldsChanged_ShouldReturnAllChanges(ChangeTrackingService service)
    {
        // Arrange
        var existingLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            TrainingCode = "123",
            LearningType = LearningType.Apprenticeship,
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF123"
        };

        var newLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001, // Keep Ukprn the same since it's the natural key
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Dob = new DateTime(1991, 2, 2),
            AcademicYear = 2025,
            StartDate = new DateTime(2025, 9, 1),
            PlannedEndDate = new DateTime(2027, 8, 31),
            PercentageLearningToBeDelivered = 80,
            EpaoPrice = 600,
            TrainingPrice = 16000,
            AgreementId = "XYZ789",
            TrainingCode = "456",
            LearningType = LearningType.ApprenticeshipUnit,
            IsFlexiJob = true,
            PlannedOTJTrainingHours = 1400,
            ReceivedDate = DateTime.UtcNow.AddDays(1),
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF456"
        };

        // Act
        var result = service.DetectChanges(existingLearner, newLearner);

        // Assert
        result.Should().NotBeNull();
        result.HasLearnerChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(11);

        // Verify each change type
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.FirstNameChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.LastNameChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.EmailChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.DobChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.StartDateChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.PlannedEndDateChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.EpaoPriceChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.TrainingPriceChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.TrainingCodeChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.LearningTypeChange);
        result.Changes.Should().ContainSingle(c => c.ChangeType == ChangeType.IsFlexiJob);

        // All tracked fields should have changes since we changed all of them
    }

    [Test, AutoData]
    public void DetectChanges_WhenOnlyNonTrackedFieldsChanged_ShouldReturnEmptyChanges(ChangeTrackingService service)
    {
        // Arrange
        var existingLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF123"
        };

        var newLearner = new Learner
        {
            Id = 2,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = DateTime.UtcNow.AddDays(1),
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF456"
        };

        // Act
        var result = service.DetectChanges(existingLearner, newLearner);

        // Assert
        result.Should().NotBeNull();
        result.HasLearnerChanges.Should().BeFalse();
        result.Changes.Should().BeEmpty();
    }

    [Test, AutoData]
    public void DetectChanges_WhenNonLearnerFieldsChanged_ShouldReturnEmptyLearnerChangesButShow(ChangeTrackingService service)
    {
        // Arrange
        var existingLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1100,
            ReceivedDate = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF123"
        };

        var newLearner = new Learner
        {
            Id = 2,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2425,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 90,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = DateTime.UtcNow.AddDays(1),
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF456"
        };

        // Act
        var result = service.DetectChanges(existingLearner, newLearner);

        // Assert
        result.Should().NotBeNull();
        result.HasLearnerChanges.Should().BeFalse();
        result.HasMaterialChanges.Should().BeTrue();
        result.Changes.Should().BeEmpty();
    }

    [Test, AutoData]
    public void DetectChanges_WhenSingleFieldChanged_ShouldReturnOnlyThatChange(ChangeTrackingService service)
    {
        // Arrange
        var existingLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF123"
        };

        var newLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = existingLearner.ReceivedDate,
            CorrelationId = existingLearner.CorrelationId,
            ConsumerReference = "REF123"
        };

        // Act
        var result = service.DetectChanges(existingLearner, newLearner);

        // Assert
        result.Should().NotBeNull();
        result.HasLearnerChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(1);
        result.Changes.Should().ContainSingle(c => 
            c.ChangeType == ChangeType.FirstNameChange);
    }

    [Test, AutoData]
    public void DetectChanges_WhenMultipleFieldsChanged_ShouldReturnAllChanges(ChangeTrackingService service)
    {
        // Arrange
        var existingLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF123"
        };

        var newLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = existingLearner.ReceivedDate,
            CorrelationId = existingLearner.CorrelationId,
            ConsumerReference = "REF123"
        };

        // Act
        var result = service.DetectChanges(existingLearner, newLearner);

        // Assert
        result.Should().NotBeNull();
        result.HasLearnerChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(3);

        result.Changes.Should().ContainSingle(c => 
            c.ChangeType == ChangeType.FirstNameChange);
        
        result.Changes.Should().ContainSingle(c => 
            c.ChangeType == ChangeType.LastNameChange);
        
        result.Changes.Should().ContainSingle(c => 
            c.ChangeType == ChangeType.EmailChange);
    }

    [Test, AutoData]
    public void DetectChanges_WhenNullableFieldChangedFromNullToValue_ShouldDetectChange(ChangeTrackingService service)
    {
        // Arrange
        var existingLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = null,
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = null,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF123"
        };

        var newLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 80,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = existingLearner.ReceivedDate,
            CorrelationId = existingLearner.CorrelationId,
            ConsumerReference = "REF123"
        };

        // Act
        var result = service.DetectChanges(existingLearner, newLearner);

        // Assert
        result.Should().NotBeNull();
        result.HasLearnerChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(1);

        result.Changes.Should().Contain(c => 
            c.ChangeType == ChangeType.EmailChange);
    }

    [Test, AutoData]
    public void DetectChanges_WhenNullableFieldChangedFromValueToNull_ShouldDetectChange(ChangeTrackingService service)
    {
        // Arrange
        var existingLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = 100,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
            ConsumerReference = "REF123"
        };

        var newLearner = new Learner
        {
            Id = 1,
            Uln = 1234567890,
            Ukprn = 10000001,
            FirstName = "John",
            LastName = "Doe",
            Email = null,
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 8, 31),
            PercentageLearningToBeDelivered = null,
            EpaoPrice = 500,
            TrainingPrice = 15000,
            AgreementId = "ABC123",
            IsFlexiJob = false,
            PlannedOTJTrainingHours = 1200,
            ReceivedDate = existingLearner.ReceivedDate,
            CorrelationId = existingLearner.CorrelationId,
            ConsumerReference = "REF123"
        };

        // Act
        var result = service.DetectChanges(existingLearner, newLearner);

        // Assert
        result.Should().NotBeNull();
        result.HasLearnerChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(1);

        result.Changes.Should().Contain(c => 
            c.ChangeType == ChangeType.EmailChange);
    }
} 