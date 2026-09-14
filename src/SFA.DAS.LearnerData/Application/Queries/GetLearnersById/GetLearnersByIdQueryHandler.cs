using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetLearnersById;

public record GetLearnersByIdQuery(long ukprn, IEnumerable<long> ids) : IRequest<List<GetLearnersByIdResultItem>>;

public class GetLearnersByIdQueryHandler(ILearnerRepository repository) : IRequestHandler<GetLearnersByIdQuery, List<GetLearnersByIdResultItem>>
{
    public async Task<List<GetLearnersByIdResultItem>> Handle(GetLearnersByIdQuery request, CancellationToken cancellationToken)
    {
        var learners = await repository.GetByIds(request.ukprn, request.ids, cancellationToken);

        return learners.Select(learner => new GetLearnersByIdResultItem
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
            TrainingCode = learner.TrainingCode,
            TrainingName = learner.TrainingName,
            LearningType = learner.LearningType,
            IsFlexiJob = learner.IsFlexiJob,
            PlannedOTJTrainingHours = learner.PlannedOTJTrainingHours,
            ApprenticeshipId = learner.ApprenticeshipId
        }).ToList();
    }
}