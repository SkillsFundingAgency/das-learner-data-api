using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.LearnerData.Data.Entities;

namespace SFA.DAS.LearnerData.Data.Repositories;

public interface ILearnerDataRepository
{
    Task<Learner> GetById(long id, CancellationToken cancellationToken);
    Task<Learner> Get(long ukPrn, long uln, int standardCode, int academicYear, CancellationToken cancellationToken);
    Task<List<Learner>> GetForProvider(long ukprn, CancellationToken cancellationToken);
    Task<PagedResult<Learner>> Search(long ukprn, int academicYear, int page, int? pageSize, int limit, int offset, string sortColumn, bool sortDescending, string filter, CancellationToken cancellationToken);
    Task<DateTime?> GetLastSubmissionDate(long ukprn, int academicYear, CancellationToken cancellationToken);
    Task<long> Save(SaveLearnerCommand request, CancellationToken cancellationToken);
}

public class LearnerDataRepository(LearnerDataDbContext dbContext) : ILearnerDataRepository
{
    public async Task<Learner> GetById(long id, CancellationToken cancellationToken)
    {
        return await dbContext.Learners.FindAsync(keyValues: [id], cancellationToken);
    }

    public async Task<Learner> Get(long ukPrn, long uln, int standardCode, int academicYear, CancellationToken cancellationToken)
    {
        return await dbContext.Learners
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

    public async Task<PagedResult<Learner>> Search(long ukprn, int academicYear, int page, int? pageSize, int limit, int offset, string sortColumn, bool sortDescending, string filter, CancellationToken cancellationToken)
    {
        var query = dbContext.Learners
            .AsNoTracking()
            .Where(x => x.Ukprn == ukprn && x.AcademicYear == academicYear);

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.LastName == filter | x.Uln.ToString() == filter);
        }

        if (string.IsNullOrEmpty(sortColumn))
        {
            sortColumn = nameof(Learner.LastName);
        }

        query = sortDescending ? query.OrderByDescending(GetOrderByField(sortColumn)) : query.OrderBy(GetOrderByField(sortColumn));

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

    private static Expression<Func<Learner, object>> GetOrderByField(string fieldName)
    {
        switch (fieldName)
        {
            case nameof(Learner.AgreementId):
                return learner => learner.AgreementId;
            case nameof(Learner.Dob):
                return learner => learner.Dob;
            case nameof(Learner.Email):
                return learner => learner.Email;
            case nameof(Learner.EpaoPrice):
                return learner => learner.EpaoPrice;
            case nameof(Learner.FirstName):
                return learner => learner.FirstName;
            case nameof(Learner.IsFlexiJob):
                return learner => learner.IsFlexiJob;
            case nameof(Learner.LastName):
                return learner => learner.LastName;
            case nameof(Learner.PercentageLearningToBeDelivered):
                return learner => learner.PercentageLearningToBeDelivered;
            case nameof(Learner.PlannedOTJTrainingHours):
                return learner => learner.PlannedOTJTrainingHours;
            case nameof(Learner.PlannedEndDate):
                return learner => learner.PlannedEndDate;
            case nameof(Learner.ReceivedDate):
                return learner => learner.ReceivedDate;
            case nameof(Learner.StandardCode):
                return learner => learner.StandardCode;
            case nameof(Learner.StartDate):
                return learner => learner.StartDate;
            case nameof(Learner.TrainingPrice):
                return learner => learner.TrainingPrice;
            case nameof(Learner.Uln):
                return learner => learner.Uln;
            default:
                return learner => learner.LastName;
        }
    }

    public async Task<DateTime?> GetLastSubmissionDate(long ukprn, int academicYear, CancellationToken cancellationToken)
    {
        return await dbContext.Learners
            .AsNoTracking()
            .Where(x => x.Ukprn == ukprn && x.AcademicYear == academicYear)
            .Select(x=> x.ReceivedDate)
            .OrderByDescending(x=> x)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<long> Save(SaveLearnerCommand request, CancellationToken cancellationToken)
    {
        var existingLearner = await Get(request.Ukprn, request.Uln, request.StandardCode, request.AcademicYear, cancellationToken);

        if (existingLearner == null)
        {
            var learner = Learner.From(request);
            await dbContext.Learners.AddAsync(learner, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return learner.Id;
        }

        existingLearner.Uln = request.Uln;
        existingLearner.Ukprn = request.Ukprn;
        existingLearner.FirstName = request.FirstName;
        existingLearner.LastName = request.LastName;
        existingLearner.Email = request.Email;
        existingLearner.Dob = request.Dob;
        existingLearner.AcademicYear = request.AcademicYear;
        existingLearner.StartDate = request.StartDate;
        existingLearner.PlannedEndDate = request.PlannedEndDate;
        existingLearner.PercentageLearningToBeDelivered = request.PercentageLearningToBeDelivered;
        existingLearner.EpaoPrice = request.EpaoPrice;
        existingLearner.TrainingPrice = request.TrainingPrice;
        existingLearner.AgreementId = request.AgreementId;
        existingLearner.ConsumerReference = request.ConsumerReference;
        existingLearner.CorrelationId = request.CorrelationId;
        existingLearner.ReceivedDate = request.ReceivedDate;
        existingLearner.StandardCode = request.StandardCode;
        existingLearner.IsFlexiJob = request.IsFlexiJob;
        existingLearner.PlannedOTJTrainingHours = request.PlannedOTJTrainingHours;

        await dbContext.SaveChangesAsync(cancellationToken);

        return existingLearner.Id;
    }
}