using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetByAcademicYear;

public record GetByAcademicYearQuery : PagedQuery, IRequest<GetByAcademicYearResult>
{
    public long UkPrn { get; }
    public int AcademicYear { get; }

    public GetByAcademicYearQuery(long ukPrn, int academicYear, int page, int? pageSize)
    {
        UkPrn = ukPrn;
        AcademicYear = academicYear;
        Page = page;
        PageSize = pageSize;
    }
}

public class GetByAcademicYearQueryHandler(ILearnerDataRepository repository): IRequestHandler<GetByAcademicYearQuery, GetByAcademicYearResult>
{
    public async Task<GetByAcademicYearResult> Handle(GetByAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByAcademicYear(
            request.UkPrn,
            request.AcademicYear,
            request.Page,
            request.PageSize,
            request.Limit,
            request.Offset,
            cancellationToken);

        return new GetByAcademicYearResult
        {
            Items = result.Data.Select(learner => new GetByAcademicYearResultItem
            {
                Id = learner.Id,
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