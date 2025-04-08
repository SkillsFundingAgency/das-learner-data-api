using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetLearner;

public record GetLearnerQuery(long Ukprn, long Uln, int StandardCode, int AcademicYear) : IRequest<GetLearnerResult>;

public class GetLearnerQueryHandler(ILearnerDataRepository repository): IRequestHandler<GetLearnerQuery, GetLearnerResult>
{
    public async Task<GetLearnerResult> Handle(GetLearnerQuery request, CancellationToken cancellationToken)
    {
        var learner = await repository.Get(request.Ukprn, request.Uln,request.StandardCode, request.AcademicYear, cancellationToken);

        if (learner == null)
        {
            return new GetLearnerResult();
        }

        return new GetLearnerResult
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
        };
    }
}