using SFA.DAS.LearnerData.Data.Entities;

namespace SFA.DAS.LearnerData.Data.Repositories;

public interface ILearnerDataRepository
{
    Task Create(Learner? learner, CancellationToken cancellationToken);
    Task<Learner?> GetById(long id, CancellationToken cancellationToken);
}

public class LearnerDataRepository(LearnerDataDbContext dbContext) : ILearnerDataRepository
{
    public async Task Create(Learner? learner, CancellationToken cancellationToken)
    {
        await dbContext.Learners.AddAsync(learner, cancellationToken);
    }

    public async Task<Learner?> GetById(long id, CancellationToken cancellationToken)
    {
        return await dbContext.Learners.FindAsync(keyValues: [id], cancellationToken);
    }
}