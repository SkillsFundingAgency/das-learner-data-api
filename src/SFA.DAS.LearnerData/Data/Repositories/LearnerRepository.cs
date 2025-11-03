using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.LearnerData.Data.Entities;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.LearnerData.Data.Repositories;

public interface ILearnerRepository
{
    Task<Learner?> GetById(long id, CancellationToken cancellationToken);
    Task<Learner?> Get(long ukPrn, long uln, int standardCode, int academicYear, CancellationToken cancellationToken);
    Task<Learner?> Get(long ukPrn, long uln, CancellationToken cancellationToken);
    Task<List<Learner>> GetForProvider(long ukprn, CancellationToken cancellationToken);

    Task<PagedResult<Learner>> Search(long ukprn, int page, int? pageSize, int limit, int offset,
        string sortColumn, bool sortDescending, string filter, bool excludeApproved, int? startMonth, int startYear,
        CancellationToken cancellationToken);
    Task<PagedResult<Learner>> GetAllLearners(int page, int? pageSize, int limit, int offset, bool excludeApproved, CancellationToken cancellationToken);
    Task<DateTime?> GetLastSubmissionDate(long ukprn, CancellationToken cancellationToken);
    Task<SaveLearnerCommandResponse> Save(SaveLearnerCommand request, CancellationToken cancellationToken);
    Task<SaveLearnerNewCommandResponse> AddLearner(SaveLearnerNewCommand request, CancellationToken cancellationToken);
    Task<SaveLearnerNewCommandResponse> UpdateLearner(Learner existingLearner, SaveLearnerNewCommand request, CancellationToken cancellationToken);
    Task AssignApprenticeshipId(AssignApprenticeshipIdCommand request, CancellationToken cancellationToken);
}

public class LearnerRepository(LearnerDataDbContext dbContext, ILogger<LearnerRepository> logger) : ILearnerRepository
{
    public async Task<Learner?> GetById(long id, CancellationToken cancellationToken)
    {
        return await dbContext.Learners.FindAsync(keyValues: [id], cancellationToken);
    }

    public async Task<Learner?> Get(long ukPrn, long uln, int standardCode, int academicYear, CancellationToken cancellationToken)
    {
        return await dbContext.Learners
            .SingleOrDefaultAsync(learner => learner.Ukprn == ukPrn
                                             && learner.Uln == uln
                                             && learner.StandardCode == standardCode
                                             && learner.AcademicYear == academicYear
                , cancellationToken);
    }

    public async Task<Learner?> Get(long ukPrn, long uln, CancellationToken cancellationToken)
    {
        return await dbContext.Learners.OrderBy(x=>x.Ukprn).ThenBy(x=>x.Uln).ThenByDescending(x=>x.Id)
            .FirstOrDefaultAsync(learner => learner.Ukprn == ukPrn
                                             && learner.Uln == uln
                , cancellationToken);
    }

    public async Task<List<Learner>> GetForProvider(long ukprn, CancellationToken cancellationToken)
    {
        return await dbContext.Learners
            .AsNoTracking()
            .Where(x => x.Ukprn == ukprn)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Learner>> Search(long ukprn, int page, int? pageSize, int limit, int offset, string sortColumn, 
        bool sortDescending, string filter, bool excludeApproved, int? startMonth, int startYear, CancellationToken cancellationToken)
    {
        var query = dbContext.Learners
            .AsNoTracking()
            .Where(x => x.Ukprn == ukprn);

        if (excludeApproved)
        {
            query = query.Where(x => x.ApprenticeshipId == null);
        }

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.LastName.Contains(filter) || x.FirstName.Contains(filter) || x.Uln.ToString() == filter);
        }

        if (startMonth.HasValue)
        {
            var month = startMonth.Value;
            if (month >= 1 && month <= 12)
            {
                query = query.Where(x => x.StartDate.Month == month);
            }
        }

        if (startYear > 0)
        {
            query = query.Where(x => x.StartDate.Year == startYear);
        }

        if (string.IsNullOrEmpty(sortColumn))
        {
            sortColumn = nameof(Learner.StartDate);
            sortDescending = true;
        }

        query = query.OrderBy(GetOrderNamesByField(sortColumn, sortDescending));

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

    public async Task<PagedResult<Learner>> GetAllLearners(int page, int? pageSize, int limit, int offset, bool excludeApproved, CancellationToken cancellationToken)
    {
        var query = dbContext.Learners
            .AsNoTracking();

        if (excludeApproved)
        {
            query = query.Where(x => x.ApprenticeshipId == null);
        }

        query = query.OrderBy(x => x.Id);

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

    private static string GetOrderNamesByField(string fieldName, bool sortDescending)
    {
        var sort = sortDescending ? "descending" : "ascending";

        return fieldName switch
        {
            nameof(Learner.AgreementId) => $"AgreementId {sort}",
            nameof(Learner.Dob) => $"Dob  {sort}",
            nameof(Learner.Email) => $"Email  {sort}",
            nameof(Learner.EpaoPrice) => $"EpaoPrice  {sort}",
            nameof(Learner.FirstName) => $"Firstname  {sort}, Lastname  {sort}",
            nameof(Learner.IsFlexiJob) => $"IsFlexiJob  {sort}",
            nameof(Learner.LastName) => $"Lastname  {sort}, Firstname  {sort}",
            nameof(Learner.PercentageLearningToBeDelivered) => $"PercentageLearningToBeDelivered  {sort}",
            nameof(Learner.PlannedOTJTrainingHours) => $"PlannedOTJTrainingHours  {sort}",
            nameof(Learner.PlannedEndDate) => $"PlannedEndDate  {sort}",
            nameof(Learner.ReceivedDate) => $"ReceivedDate  {sort}",
            nameof(Learner.StandardCode) => $"StandardCode  {sort}",
            nameof(Learner.StartDate) => $"StartDate  {sort}, Firstname  {sort}, Lastname  {sort}, ULN  {sort}",
            nameof(Learner.TrainingPrice) => $"TrainingPrice  {sort}",
            nameof(Learner.Uln) => $"Uln  {sort}",
            _ => $"Lastname  {sort}, Firstname  {sort}"
        };
    }

    public async Task<DateTime?> GetLastSubmissionDate(long ukprn, CancellationToken cancellationToken)
    {
        var query = dbContext.Learners
            .Where(x => x.Ukprn == ukprn)
            .AsNoTracking();

        return await query
            .Select(x => x.ReceivedDate)
            .OrderByDescending(x => x)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<SaveLearnerCommandResponse> Save(SaveLearnerCommand request, CancellationToken cancellationToken)
    {
        var existingLearner = await Get(request.Ukprn, request.Uln, request.StandardCode, request.AcademicYear,
            cancellationToken);

        if (existingLearner == null)
        {
            var learner = Learner.From(request);
            await dbContext.Learners.AddAsync(learner, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new SaveLearnerCommandResponse { Id = learner.Id, Result = SaveLearnerResult.Created };
        }

        if (existingLearner.ApprenticeshipId != null)
        {
            logger.LogError("Learner record {0} cannot be updated as it already has an ApprenticeshipId assigned", existingLearner.Id);
            throw new InvalidOperationException($"Learner with ID {existingLearner.Id} already has ApprenticeshipId {existingLearner.ApprenticeshipId} assigned. Cannot update.");
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

    public async Task<SaveLearnerNewCommandResponse> AddLearner(SaveLearnerNewCommand request, CancellationToken cancellationToken)
    {
        var learner = Learner.From(request);
        await dbContext.Learners.AddAsync(learner, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SaveLearnerNewCommandResponse { Id = learner.Id, Result = SaveLearnerNewResult.Created };
    }

    public async Task<SaveLearnerNewCommandResponse> UpdateLearner(Learner existingLearner, SaveLearnerNewCommand request, CancellationToken cancellationToken)
    {

        if (existingLearner.ApprenticeshipId != null)
        {
            logger.LogError("Learner record {0} cannot be updated as it already has an ApprenticeshipId assigned", existingLearner.Id);
            throw new InvalidOperationException($"Learner with ID {existingLearner.Id} already has ApprenticeshipId {existingLearner.ApprenticeshipId} assigned. Cannot update.");
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

        return new SaveLearnerNewCommandResponse { Id = existingLearner.Id, Result = SaveLearnerNewResult.Updated };
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
}