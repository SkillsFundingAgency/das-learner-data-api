using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetAll;

public record GetForProviderQuery(long Ukprn): IRequest<GetForProviderResult>;

public class GetForProviderQueryHandler(ILearnerRepository repository): IRequestHandler<GetForProviderQuery, GetForProviderResult>
{
    public async Task<GetForProviderResult> Handle(GetForProviderQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetForProvider(request.Ukprn, cancellationToken);

        if (result == null || result.Count == 0)
        {
            return new GetForProviderResult();
        }

        return new GetForProviderResult
        {
            Learners = result.Select(learner => new GetForProviderResultItem
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
                IsFlexiJob = learner.IsFlexiJob,
                PlannedOTJTrainingHours = learner.PlannedOTJTrainingHours
            }).ToList()
        };
    }
}