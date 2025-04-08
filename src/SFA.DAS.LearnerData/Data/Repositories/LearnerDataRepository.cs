using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerData.Data.Entities;

namespace SFA.DAS.LearnerData.Data.Repositories;

public interface ILearnerDataRepository
{
    Task Create(Learner? learner, CancellationToken cancellationToken);
    Task<Learner?> GetById(long id, CancellationToken cancellationToken);
    Task<Learner?> Get(long ukPrn, long uln, string agreementId, int academicYear, CancellationToken cancellationToken);
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

    public async Task<Learner?> Get(long ukPrn, long uln, string agreementId, int academicYear, CancellationToken cancellationToken)
    {
        return await dbContext.Learners
            .AsNoTracking()
            .SingleOrDefaultAsync(learner => learner.Ukprn == ukPrn
                                             && learner.Uln == uln
                                             && learner.AgreementId == agreementId
                                             && learner.AcademicYear == academicYear
                , cancellationToken);
    }
}