using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetLearnerById;

public record GetLearnerByIdQuery(long ukprn, long Id) : IRequest<GetLearnerByIdResult>;

public class GetLearnerByIdQueryHandler(ILearnerRepository repository) : IRequestHandler<GetLearnerByIdQuery, GetLearnerByIdResult>
{
    public async Task<GetLearnerByIdResult> Handle(GetLearnerByIdQuery request, CancellationToken cancellationToken)
    {
        var learner = await repository.GetById(request.Id, cancellationToken);

        if (learner == null || learner.Ukprn != request.ukprn)
        {
            return new GetLearnerByIdResult();
        }

        return new GetLearnerByIdResult
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
            PlannedOTJTrainingHours = learner.PlannedOTJTrainingHours,
            ApprenticeshipId = learner.ApprenticeshipId
        };
    }
}