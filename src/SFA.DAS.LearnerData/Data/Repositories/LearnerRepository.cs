using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.LearnerData.Data.Entities;

namespace SFA.DAS.LearnerData.Data.Repositories;

public interface ILearnerRepository
{
    Task<Learner?> GetById(long id, CancellationToken cancellationToken);
    Task<Learner> Get(long ukPrn, long uln, int standardCode, int academicYear, CancellationToken cancellationToken);
    Task<List<Learner>> GetForProvider(long ukprn, CancellationToken cancellationToken);

    Task<PagedResult<Learner>> Search(long ukprn, int? academicYear, int page, int? pageSize, int limit, int offset,
        string sortColumn, bool sortDescending, string filter, bool excludeUnapproved,
        CancellationToken cancellationToken);
    Task<DateTime?> GetLastSubmissionDate(long ukprn, int? academicYear, CancellationToken cancellationToken);
    Task<SaveLearnerCommandResponse> Save(SaveLearnerCommand request, CancellationToken cancellationToken);
    Task AssignApprenticeshipId(AssignApprenticeshipIdCommand request, CancellationToken cancellationToken);
}

public class LearnerRepository(LearnerDataDbContext dbContext) : ILearnerRepository
{
    public async Task<Learner?> GetById(long id, CancellationToken cancellationToken)
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

    public async Task<PagedResult<Learner>> Search(long ukprn, int? academicYear, int page, int? pageSize, int limit, int offset, string sortColumn, 
        bool sortDescending, string filter, bool excludeUnapproved, CancellationToken cancellationToken)
    {
        var query = dbContext.Learners
            .AsNoTracking()
            .Where(x => x.Ukprn == ukprn);

        if (excludeUnapproved)
        {
            query = query.Where(x => x.ApprenticeshipId == null);
        }

        if (academicYear.HasValue)
        {
            query = query.Where(x => x.AcademicYear == academicYear);
        }

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.LastName.Contains(filter) || x.FirstName.Contains(filter) || x.Uln.ToString() == filter);
        }

        if (string.IsNullOrEmpty(sortColumn))
        {
            sortColumn = nameof(Learner.FirstName);
        }

        query = sortDescending ? query.OrderByDescending(GetOrderByField(sortColumn)).ThenByDescending(GetSecondarySortByField(sortColumn)) 
            : query.OrderBy(GetOrderByField(sortColumn)).ThenBy(GetSecondarySortByField(sortColumn));

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
        return fieldName switch
        {
            nameof(Learner.AgreementId) => learner => learner.AgreementId,
            nameof(Learner.Dob) => learner => learner.Dob,
            nameof(Learner.Email) => learner => learner.Email,
            nameof(Learner.EpaoPrice) => learner => learner.EpaoPrice,
            nameof(Learner.FirstName) => learner => learner.FirstName,
            nameof(Learner.IsFlexiJob) => learner => learner.IsFlexiJob,
            nameof(Learner.LastName) => learner => learner.LastName,
            nameof(Learner.PercentageLearningToBeDelivered) => learner => learner.PercentageLearningToBeDelivered,
            nameof(Learner.PlannedOTJTrainingHours) => learner => learner.PlannedOTJTrainingHours,
            nameof(Learner.PlannedEndDate) => learner => learner.PlannedEndDate,
            nameof(Learner.ReceivedDate) => learner => learner.ReceivedDate,
            nameof(Learner.StandardCode) => learner => learner.StandardCode,
            nameof(Learner.StartDate) => learner => learner.StartDate,
            nameof(Learner.TrainingPrice) => learner => learner.TrainingPrice,
            nameof(Learner.Uln) => learner => learner.Uln,
            _ => learner => learner.LastName
        };
    }

    public async Task<DateTime?> GetLastSubmissionDate(long ukprn, int? academicYear, CancellationToken cancellationToken)
    {
        var query = dbContext.Learners
            .Where(x => x.Ukprn == ukprn)
            .AsNoTracking();

        if (academicYear.HasValue)
        {
            query = query.Where(x => x.AcademicYear == academicYear.Value);
        }

        return await query
            .Select(x => x.ReceivedDate)
            .OrderByDescending(x => x)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<SaveLearnerCommandResponse> Save(SaveLearnerCommand request, CancellationToken cancellationToken)
    {
        var existingLearner = await Get(request.Ukprn, request.Uln, request.StandardCode, request.AcademicYear, cancellationToken);

        if (existingLearner == null)
        {
            var learner = Learner.From(request);
            await dbContext.Learners.AddAsync(learner, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new SaveLearnerCommandResponse { Id = learner.Id, Result = SaveLearnerResult.Created };
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

        return new SaveLearnerCommandResponse { Id = existingLearner.Id, Result = SaveLearnerResult.Updated };
    }

    public async Task AssignApprenticeshipId(AssignApprenticeshipIdCommand request, CancellationToken cancellationToken)
    {
        var learner = await GetById(request.LearnerDataId, cancellationToken);
        if (learner == null)
        {
            throw new KeyNotFoundException($"Learner with ID {request.LearnerDataId} not found.");
        }
        if (learner.Ukprn != request.Ukprn)
        {
            throw new KeyNotFoundException($"Learner with ID {request.LearnerDataId} not found for UKPRN {request.Ukprn}");
        }

        if (learner.ApprenticeshipId != null && learner.ApprenticeshipId != request.ApprenticeshipId)
        {
            throw new InvalidOperationException($"Learner with ID {request.LearnerDataId} already has a different ApprenticeshipId assigned.");
        }

        learner.ApprenticeshipId = request.ApprenticeshipId;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    protected static Expression<Func<Learner, object>> GetSecondarySortByField(string fieldName)
    {
        switch (fieldName)
        {
            case nameof(Learner.FirstName):
                return apprenticeship => apprenticeship.LastName;
            default:
                return GetOrderByField(fieldName);
        }
    }
}