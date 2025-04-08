using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerData.Data.Entities;

namespace SFA.DAS.LearnerData.Data.Repositories;

public interface ILearnerDataRepository
{
    Task Create(Learner? learner, CancellationToken cancellationToken);
    Task<Learner?> GetById(long id, CancellationToken cancellationToken);
    Task<Learner?> Get(long ukPrn, long uln, int standardCode, int academicYear, CancellationToken cancellationToken);
    Task<List<Learner>> GetForProvider(long ukprn, CancellationToken cancellationToken);
    Task<PagedResult<Learner>> GetByAcademicYear(long ukprn, int academicYear, int page, int? pageSize, int limit, int offset, CancellationToken cancellationToken);
    Task<DateTime?> GetLastSubmissionDate(long ukprn, int academicYear, CancellationToken cancellationToken);
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

    public async Task<Learner?> Get(long ukPrn, long uln, int standardCode, int academicYear, CancellationToken cancellationToken)
    {
        return await dbContext.Learners
            .AsNoTracking()
            .SingleOrDefaultAsync(learner => learner.Ukprn == ukPrn
                                             && learner.Uln == uln
                                             && learner.StandardCode == standardCode
                                             && learner.AcademicYear == academicYear
                , cancellationToken);
    }

    public async Task<List<Learner>> GetForProvider(long ukprn, CancellationToken cancellationToken)
    {
        return await dbContext.Learners
            .AsNoTracking()
            .Where(x => x.Ukprn == ukprn)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Learner>> GetByAcademicYear(long ukprn, int academicYear, int page, int? pageSize, int limit, int offset, CancellationToken cancellationToken)
    {
        var query = dbContext.Learners
            .AsNoTracking()
            .Where(x => x.Ukprn == ukprn && x.AcademicYear == academicYear);

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize.GetValueOrDefault());
        
        var result = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return new PagedResult<Learner>
        {
            Data = result,
            TotalItems = totalItems,
            TotalPages = totalPages,
            PageSize = pageSize ?? int.MaxValue,
            Page = page,
        };
    }

    public async Task<DateTime?> GetLastSubmissionDate(long ukprn, int academicYear, CancellationToken cancellationToken)
    {
        return await dbContext.Learners
            .AsNoTracking()
            .Where(x => x.Ukprn == ukprn && x.AcademicYear == academicYear)
            .MaxAsync(x => x.CreatedDate, cancellationToken);
    }
}