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
            StandardCode = 123,
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
            StandardCode = 123,
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
        result.HasChanges.Should().BeFalse();
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
            StandardCode = 123,
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
            Ukprn = 10000002,
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
            StandardCode = 456,
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
        result.HasChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(15);

        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.Ukprn) && c.OldValue.Equals(10000001L) && c.NewValue.Equals(10000002L));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.FirstName) && c.OldValue.Equals("John") && c.NewValue.Equals("Jane"));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.LastName) && c.OldValue.Equals("Doe") && c.NewValue.Equals("Smith"));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.Email) && c.OldValue.Equals("john.doe@example.com") && c.NewValue.Equals("jane.smith@example.com"));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.Dob) && c.OldValue.Equals(new DateTime(1990, 1, 1)) && c.NewValue.Equals(new DateTime(1991, 2, 2)));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.AcademicYear) && c.OldValue.Equals(2024) && c.NewValue.Equals(2025));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.StartDate) && c.OldValue.Equals(new DateTime(2024, 9, 1)) && c.NewValue.Equals(new DateTime(2025, 9, 1)));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.PlannedEndDate) && c.OldValue.Equals(new DateTime(2026, 8, 31)) && c.NewValue.Equals(new DateTime(2027, 8, 31)));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.PercentageLearningToBeDelivered) && c.OldValue.Equals(100) && c.NewValue.Equals(80));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.EpaoPrice) && c.OldValue.Equals(500) && c.NewValue.Equals(600));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.TrainingPrice) && c.OldValue.Equals(15000) && c.NewValue.Equals(16000));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.AgreementId) && c.OldValue.Equals("ABC123") && c.NewValue.Equals("XYZ789"));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.StandardCode) && c.OldValue.Equals(123) && c.NewValue.Equals(456));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.IsFlexiJob) && c.OldValue.Equals(false) && c.NewValue.Equals(true));
        result.Changes.Should().Contain(c => c.FieldName == nameof(Learner.PlannedOTJTrainingHours) && c.OldValue.Equals(1200) && c.NewValue.Equals(1400));

        result.Changes.Should().NotContain(c => c.FieldName == nameof(Learner.Uln));
        result.Changes.Should().NotContain(c => c.FieldName == nameof(Learner.ReceivedDate));
        result.Changes.Should().NotContain(c => c.FieldName == nameof(Learner.CorrelationId));
        result.Changes.Should().NotContain(c => c.FieldName == nameof(Learner.ConsumerReference));
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
            StandardCode = 123,
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
            StandardCode = 123,
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
        result.HasChanges.Should().BeFalse();
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
            StandardCode = 123,
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
            StandardCode = 123,
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
        result.HasChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(1);
        result.Changes.Should().ContainSingle(c => 
            c.FieldName == nameof(Learner.FirstName) && 
            c.OldValue.Equals("John") && 
            c.NewValue.Equals("Jane"));
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
            StandardCode = 123,
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
            StandardCode = 123,
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
        result.HasChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(3);

        result.Changes.Should().Contain(c => 
            c.FieldName == nameof(Learner.FirstName) && 
            c.OldValue.Equals("John") && 
            c.NewValue.Equals("Jane"));
        
        result.Changes.Should().Contain(c => 
            c.FieldName == nameof(Learner.LastName) && 
            c.OldValue.Equals("Doe") && 
            c.NewValue.Equals("Smith"));
        
        result.Changes.Should().Contain(c => 
            c.FieldName == nameof(Learner.Email) && 
            c.OldValue.Equals("john.doe@example.com") && 
            c.NewValue.Equals("jane.smith@example.com"));
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
            StandardCode = 123,
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
            StandardCode = 123,
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
        result.HasChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(2);

        result.Changes.Should().Contain(c => 
            c.FieldName == nameof(Learner.Email) && 
            c.OldValue == null && 
            c.NewValue.Equals("john.doe@example.com"));
        
        result.Changes.Should().Contain(c => 
            c.FieldName == nameof(Learner.PercentageLearningToBeDelivered) && 
            c.OldValue == null && 
            c.NewValue.Equals(80));
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
            StandardCode = 123,
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
            StandardCode = 123,
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
        result.HasChanges.Should().BeTrue();
        result.Changes.Should().HaveCount(2);

        result.Changes.Should().Contain(c => 
            c.FieldName == nameof(Learner.Email) && 
            c.OldValue.Equals("john.doe@example.com") && 
            c.NewValue == null);
        
        result.Changes.Should().Contain(c => 
            c.FieldName == nameof(Learner.PercentageLearningToBeDelivered) && 
            c.OldValue.Equals(100) && 
            c.NewValue == null);
    }
} 