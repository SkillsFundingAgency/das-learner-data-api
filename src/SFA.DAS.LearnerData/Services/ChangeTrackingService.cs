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

        if (!Equals(existingLearner.Dob, newLearner.Dob))
        {
            changes.Add(new DobChange { OldValue = existingLearner.Dob, NewValue = newLearner.Dob });
        }

        if (!Equals(existingLearner.StartDate, newLearner.StartDate))
        {
            changes.Add(new StartDateChange { OldValue = existingLearner.StartDate, NewValue = newLearner.StartDate });
        }

        if (!Equals(existingLearner.PlannedEndDate, newLearner.PlannedEndDate))
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

        return new ChangeSummary
        {
            Changes = changes
        };
    }
} 