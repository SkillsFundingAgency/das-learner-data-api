using MediatR;

namespace SFA.DAS.LearnerData.Application.Queries.GetAllLearners;

public record GetAllLearnersQuery : PagedQuery, IRequest<GetAllLearnersResult>
{
    public bool ExcludeApproved { get; }

    public GetAllLearnersQuery(int page, int? pageSize, bool excludeApproved)
    {
        Page = page;
        PageSize = pageSize;
        ExcludeApproved = excludeApproved;
    }
}
