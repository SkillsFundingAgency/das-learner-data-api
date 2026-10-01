using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetLearnerById;

public record GetLearnerByIdQuery(long Ukprn, long Id) : IRequest<GetLearnerByIdResult>;

public class GetLearnerByIdQueryHandler(ILearnerRepository repository) : IRequestHandler<GetLearnerByIdQuery, GetLearnerByIdResult>
{
    public async Task<GetLearnerByIdResult> Handle(GetLearnerByIdQuery request, CancellationToken cancellationToken)
    {
        var learner = await repository.GetById(request.Id, cancellationToken);

        if (learner == null || learner.Ukprn != request.Ukprn)
        {
            return new GetLearnerByIdResult();
        }

        return GetLearnerByIdResult.MapFrom(learner);
    }
}
