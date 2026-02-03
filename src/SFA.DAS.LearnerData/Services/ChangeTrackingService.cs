using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Messages;

namespace SFA.DAS.LearnerData.Services;

public class ChangeTrackingService : IChangeTrackingService
{
    public ChangeSummary DetectChanges(Learner existingLearner, Learner newLearner)
    {
        var changes = new List<IChange>();

        if (!Equals(existingLearner.FirstName, newLearner.FirstName))
        {
            changes.Add(new FirstNameChange { OldValue = existingLearner.FirstName, NewValue = newLearner.FirstName });
        }

        if (!Equals(existingLearner.LastName, newLearner.LastName))
        {
            changes.Add(new LastNameChange { OldValue = existingLearner.LastName, NewValue = newLearner.LastName });
        }

        if (!Equals(existingLearner.Email, newLearner.Email))
        {
            changes.Add(new EmailChange { OldValue = existingLearner.Email, NewValue = newLearner.Email });
        }

        if (!Equals(existingLearner.Dob.Date, newLearner.Dob.Date))
        {
            changes.Add(new DobChange { OldValue = existingLearner.Dob, NewValue = newLearner.Dob });
        }

        if (!Equals(existingLearner.StartDate.Date, newLearner.StartDate.Date))
        {
            changes.Add(new StartDateChange { OldValue = existingLearner.StartDate, NewValue = newLearner.StartDate });
        }

        if (!Equals(existingLearner.PlannedEndDate.Date, newLearner.PlannedEndDate.Date))
        {
            changes.Add(new PlannedEndDateChange { OldValue = existingLearner.PlannedEndDate, NewValue = newLearner.PlannedEndDate });
        }

        if (!Equals(existingLearner.EpaoPrice, newLearner.EpaoPrice))
        {
            changes.Add(new EpaoPriceChange { OldValue = existingLearner.EpaoPrice, NewValue = newLearner.EpaoPrice });
        }

        if (!Equals(existingLearner.TrainingPrice, newLearner.TrainingPrice))
        {
            changes.Add(new TrainingPriceChange { OldValue = existingLearner.TrainingPrice, NewValue = newLearner.TrainingPrice });
        }

        if (!Equals(existingLearner.StandardCode, newLearner.StandardCode))
        {
            changes.Add(new StandardCodeChange { OldValue = existingLearner.StandardCode, NewValue = newLearner.StandardCode });
        }

        if (!Equals(existingLearner.TrainingCode, newLearner.TrainingCode))
        {
            changes.Add(new TrainingCodeChange { OldValue = existingLearner.TrainingCode, NewValue = newLearner.TrainingCode });
        }

        if (!Equals(existingLearner.LearningType, newLearner.LearningType))
        {
            changes.Add(new LearningTypeChange { OldValue = existingLearner.LearningType, NewValue = newLearner.LearningType });
        }

        if (!Equals(existingLearner.IsFlexiJob, newLearner.IsFlexiJob))
        {
            changes.Add(new IsFlexiJobChange { OldValue = existingLearner.IsFlexiJob, NewValue = newLearner.IsFlexiJob });
        }

        var otherFieldsChanged =
            !Equals(existingLearner.PercentageLearningToBeDelivered, newLearner.PercentageLearningToBeDelivered) ||
            !Equals(existingLearner.AgreementId, newLearner.AgreementId) ||
            !Equals(existingLearner.PlannedOTJTrainingHours, newLearner.PlannedOTJTrainingHours);
        return new ChangeSummary
        {
            HasMaterialChanges = changes.Any() || otherFieldsChanged,
            Changes = changes
        };
    }
} 