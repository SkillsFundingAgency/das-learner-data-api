using SFA.DAS.LearnerData.Data.Entities;

namespace SFA.DAS.LearnerData.Data.Repositories;

public interface ILearnerDataRepository
{
    Task Create(Learner learner);
}

public class LearnerDataRepository(LearnerDataDbContext dbContext) : ILearnerDataRepository
{
    public async Task Create(Learner learner)
    {
        await dbContext.Learners.AddAsync(learner);
    }
}