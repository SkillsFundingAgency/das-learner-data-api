using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetSearch;

public record GetSearchQuery : PagedQuery, IRequest<GetSearchResult>
{
    public long UkPrn { get; }
    public int? AcademicYear { get; }
    public string SortColumn { get; }
    public bool SortDescending { get; }
    public string Filter { get; }
    public bool ExcludeUnapproved { get; }

    public GetSearchQuery(long ukPrn, int? academicYear, int page, int? pageSize, string sortColumn, bool sortDescending, string filter, bool excludeUnapproved)
    {
        UkPrn = ukPrn;
        AcademicYear = academicYear;
        SortColumn = sortColumn;
        SortDescending = sortDescending;
        Filter = filter;
        Page = page;
        PageSize = pageSize;
        ExcludeUnapproved = excludeUnapproved;
    }
}

public class GetSearchQueryHandler(ILearnerRepository repository): IRequestHandler<GetSearchQuery, GetSearchResult>
{
    public async Task<GetSearchResult> Handle(GetSearchQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.Search(
            request.UkPrn,
            request.AcademicYear,
            request.Page,
            request.PageSize,
            request.Limit,
            request.Offset,
            request.SortColumn,
            request.SortDescending,
            request.Filter,
            request.ExcludeUnapproved,
            cancellationToken);

        DateTime? lastSubmissionDate = null;

        if (result.TotalItems > 0)
        {
            lastSubmissionDate = await repository.GetLastSubmissionDate(request.UkPrn, request.AcademicYear, cancellationToken);            
        }

        return new GetSearchResult
        {
            LastSubmissionDate = lastSubmissionDate,
            Items = result.Data.Select(learner => new GetByAcademicYearResultItem
            {
                Id = learner.Id,
                CreatedDate = learner.CreatedDate,
                UpdatedDate = learner.UpdatedDate,
                Uln = learner.Uln,
                Ukprn = learner.Ukprn,
                FirstName = learner.FirstName,
                LastName = learner.LastName,
                Email = learner.Email,
                Dob = learner.Dob,
                AcademicYear = learner.AcademicYear,
                StartDate = learner.StartDate,
                PlannedEndDate = learner.PlannedEndDate,
                PercentageLearningToBeDelivered = learner.PercentageLearningToBeDelivered,
                EpaoPrice = learner.EpaoPrice,
                TrainingPrice = learner.TrainingPrice,
                AgreementId = learner.AgreementId,
                ConsumerReference = learner.ConsumerReference,
                CorrelationId = learner.CorrelationId,
                ReceivedDate = learner.ReceivedDate,
                StandardCode = learner.StandardCode,
                IsFlexiJob = learner.IsFlexiJob,
                PlannedOTJTrainingHours = learner.PlannedOTJTrainingHours
            }),
            PageSize = result.PageSize,
            Page = result.Page,
            TotalItems = result.TotalItems,
        };
    }
}