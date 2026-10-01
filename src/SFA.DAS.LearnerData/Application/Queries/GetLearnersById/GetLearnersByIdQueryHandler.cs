using MediatR;
using SFA.DAS.LearnerData.Application.Queries.GetLearnerById;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetLearnersById;

public record GetLearnersByIdQuery(long ukprn, IEnumerable<long> ids) : IRequest<List<GetLearnerByIdResult>>;

public class GetLearnersByIdQueryHandler(ILearnerRepository repository) : IRequestHandler<GetLearnersByIdQuery, List<GetLearnerByIdResult>>
{
    public async Task<List<GetLearnerByIdResult>> Handle(GetLearnersByIdQuery request, CancellationToken cancellationToken)
    {
        var learners = await repository.GetByIds(request.ukprn, request.ids, cancellationToken);

        return learners.Select(GetLearnerByIdResult.MapFrom).ToList();
    }
}
