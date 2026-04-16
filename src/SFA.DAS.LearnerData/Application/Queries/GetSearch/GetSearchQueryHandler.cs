using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Messages;

namespace SFA.DAS.LearnerData.Application.Queries.GetSearch;

public record GetSearchQuery : PagedQuery, IRequest<GetSearchResult>
{
    public long UkPrn { get; }
    public string SortColumn { get; }
    public bool SortDescending { get; }
    public string Filter { get; }
    public bool ExcludeApproved { get; }
    public int? StartMonth { get; }
    public int StartYear { get; }
    public string ExcludeUlns { get; }

    public string MaxStartDate { get; }

    public string CourseCode { get; }
    public LearningType? LearningType { get; }

    public GetSearchQuery(long ukprn, SearchLearnersRequest request)
    {
        UkPrn = ukprn;
        SortColumn = request.SortColumn;
        SortDescending = request.SortDescending;
        Filter = request.Filter;
        Page = request.Page;
        PageSize = request.PageSize;
        ExcludeApproved = request.ExcludeApproved;
        StartMonth = request.StartMonth;
        StartYear = request.StartYear;
        MaxStartDate = request.MaxStartDate;
        ExcludeUlns = request.ExcludeUlns;
        CourseCode = request.CourseCode;
        LearningType = request.LearningType;
    }
}

public class GetSearchQueryHandler(ILearnerRepository repository) : IRequestHandler<GetSearchQuery, GetSearchResult>
{
    public async Task<GetSearchResult> Handle(GetSearchQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.Search(
            request.UkPrn,
            request.Page,
            request.PageSize,
            request.Limit,
            request.Offset,
            request.SortColumn,
            request.SortDescending,
            request.Filter,
            request.ExcludeApproved,
            request.StartMonth,
            request.StartYear,
            request.MaxStartDate,
            request.ExcludeUlns,
            request.CourseCode,
            request.LearningType,
            cancellationToken);

        DateTime? lastSubmissionDate = null;

        lastSubmissionDate = await repository.GetLastSubmissionDate(request.UkPrn, cancellationToken);

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
                TrainingCode = learner.TrainingCode,
                TrainingName = learner.TrainingName,
                LearningType = learner.LearningType,
                IsFlexiJob = learner.IsFlexiJob,
                PlannedOTJTrainingHours = learner.PlannedOTJTrainingHours
            }),
            PageSize = result.PageSize,
            Page = result.Page,
            TotalItems = result.TotalItems,
        };
    }
}