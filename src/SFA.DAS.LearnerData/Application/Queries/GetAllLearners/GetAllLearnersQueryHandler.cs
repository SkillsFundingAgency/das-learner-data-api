using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetAllLearners;

public class GetAllLearnersQueryHandler(ILearnerRepository repository) : IRequestHandler<GetAllLearnersQuery, GetAllLearnersResult>
{
    public async Task<GetAllLearnersResult> Handle(GetAllLearnersQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllLearners(
            request.Page,
            request.PageSize,
            request.Limit,
            request.Offset,
            request.ExcludeApproved,
            cancellationToken);

        return new GetAllLearnersResult
        {
            Items = result.Data.Select(learner => new GetAllLearnersResultItem
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
