using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Messages;

namespace SFA.DAS.LearnerData.Services;
 
public interface IChangeTrackingService
{
    ChangeSummary DetectChanges(Learner existingLearner, Learner newLearner);
} 