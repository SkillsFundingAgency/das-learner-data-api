using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerData.Messages;
using System.Text.Json;

namespace SFA.DAS.LearnerData.UnitTests.Messages;

[TestFixture]
public class ChangeSummaryTests
{
    [Test]
    public void Serialize_ChangeSummary_With_Different_Data_Types_Should_Work_Correctly()
    {
        // Arrange
        var changeSummary = new ChangeSummary
        {
            Changes = [
                new FirstNameChange { OldValue = "John", NewValue = "Jane" },
                new DobChange { OldValue = new DateTime(1990, 1, 1), NewValue = new DateTime(1995, 6, 15) },
                new TrainingPriceChange { OldValue = 12000, NewValue = 15000 }
            ]
        };

        // Act
        var json = JsonSerializer.Serialize(changeSummary);
        var deserialized = JsonSerializer.Deserialize<ChangeSummary>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Changes.Should().HaveCount(3);
        
        // Verify string values
        var firstNameChange = deserialized.Changes[0].Should().BeOfType<FirstNameChange>().Subject;
        firstNameChange.ChangeType.Should().Be(ChangeType.FirstNameChange);
        firstNameChange.OldValue.Should().Be("John");
        firstNameChange.NewValue.Should().Be("Jane");
        
        // Verify DateTime values
        var dobChange = deserialized.Changes[1].Should().BeOfType<DobChange>().Subject;
        dobChange.ChangeType.Should().Be(ChangeType.DobChange);
        dobChange.OldValue.Should().Be(new DateTime(1990, 1, 1));
        dobChange.NewValue.Should().Be(new DateTime(1995, 6, 15));
        
        // Verify numeric values
        var priceChange = deserialized.Changes[2].Should().BeOfType<TrainingPriceChange>().Subject;
        priceChange.ChangeType.Should().Be(ChangeType.TrainingPriceChange);
        priceChange.OldValue.Should().Be(12000);
        priceChange.NewValue.Should().Be(15000);
    }

    [Test]
    public void ChangeSummary_HasChanges_Should_Return_True_When_Changes_Exist()
    {
        // Arrange
        var changeSummary = new ChangeSummary
        {
            Changes = [new FirstNameChange { OldValue = "old", NewValue = "new" }]
        };

        // Act & Assert
        changeSummary.HasChanges.Should().BeTrue();
    }

    [Test]
    public void ChangeSummary_HasChanges_Should_Return_False_When_No_Changes()
    {
        // Arrange
        var changeSummary = new ChangeSummary { Changes = [] };

        // Act & Assert
        changeSummary.HasChanges.Should().BeFalse();
    }

    [Test]
    public void TypeSafe_Changes_Should_Handle_Null_Values_Correctly()
    {
        // Arrange
        var changeSummary = new ChangeSummary
        {
            Changes = [
                new FirstNameChange { OldValue = null, NewValue = "Jane" },
                new LastNameChange { OldValue = "Smith", NewValue = null }
            ]
        };

        // Act
        var json = JsonSerializer.Serialize(changeSummary);
        var deserialized = JsonSerializer.Deserialize<ChangeSummary>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Changes.Should().HaveCount(2);

        var firstNameChange = deserialized.Changes[0].Should().BeOfType<FirstNameChange>().Subject;
        firstNameChange.OldValue.Should().BeNull();
        firstNameChange.NewValue.Should().Be("Jane");

        var lastNameChange = deserialized.Changes[1].Should().BeOfType<LastNameChange>().Subject;
        lastNameChange.OldValue.Should().Be("Smith");
        lastNameChange.NewValue.Should().BeNull();
    }

    [Test]
    public void All_Change_Types_Should_Serialize_And_Deserialize_Correctly()
    {
        // Arrange
        var changeSummary = new ChangeSummary
        {
            Changes = [
                new FirstNameChange { OldValue = "John", NewValue = "Jane" },
                new StartDateChange { OldValue = new DateTime(2023, 9, 1), NewValue = new DateTime(2024, 9, 1) },
                new PlannedEndDateChange { OldValue = new DateTime(2025, 8, 31), NewValue = new DateTime(2026, 8, 31) },
                new EpaoPriceChange { OldValue = 400, NewValue = 500 },
                new TrainingPriceChange { OldValue = 12000, NewValue = 15000 }
            ]
        };

        // Act
        var json = JsonSerializer.Serialize(changeSummary);
        var deserialized = JsonSerializer.Deserialize<ChangeSummary>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Changes.Should().HaveCount(5);

        // Verify each change type
        deserialized.Changes[0].Should().BeOfType<FirstNameChange>();
        deserialized.Changes[1].Should().BeOfType<StartDateChange>();
        deserialized.Changes[2].Should().BeOfType<PlannedEndDateChange>();
        deserialized.Changes[3].Should().BeOfType<EpaoPriceChange>();
        deserialized.Changes[4].Should().BeOfType<TrainingPriceChange>();
    }
} 