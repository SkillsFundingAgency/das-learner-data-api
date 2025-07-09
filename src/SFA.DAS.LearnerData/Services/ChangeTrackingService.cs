using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Messages;

namespace SFA.DAS.LearnerData.Services;

public class ChangeTrackingService : IChangeTrackingService
{
    private static readonly string[] FieldsToCompare =
    [
        nameof(Learner.Ukprn),
        nameof(Learner.FirstName),
        nameof(Learner.LastName),
        nameof(Learner.Email),
        nameof(Learner.Dob),
        nameof(Learner.AcademicYear),
        nameof(Learner.StartDate),
        nameof(Learner.PlannedEndDate),
        nameof(Learner.PercentageLearningToBeDelivered),
        nameof(Learner.EpaoPrice),
        nameof(Learner.TrainingPrice),
        nameof(Learner.AgreementId),
        nameof(Learner.StandardCode),
        nameof(Learner.IsFlexiJob),
        nameof(Learner.PlannedOTJTrainingHours)
    ];

    public ChangeSummary DetectChanges(Learner existingLearner, Learner newLearner)
    {
        var changes = new List<FieldChange>();

        foreach (var fieldName in FieldsToCompare)
        {
            var existingValue = GetPropertyValue(existingLearner, fieldName);
            var newValue = GetPropertyValue(newLearner, fieldName);

            if (!Equals(existingValue, newValue))
            {
                changes.Add(new FieldChange
                {
                    FieldName = fieldName,
                    OldValue = existingValue,
                    NewValue = newValue
                });
            }
        }

        return new ChangeSummary
        {
            Changes = changes
        };
    }

    private static object? GetPropertyValue(Learner learner, string propertyName)
    {
        var property = typeof(Learner).GetProperty(propertyName);
        return property?.GetValue(learner);
    }
} 