using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Queries.GetCourseCodesByUkprn;

public record GetCourseCodesByUkprnQuery(long Ukprn) : IRequest<GetCourseCodesByUkprnResult>;

public class GetCourseCodesByUkprnQueryHandler(ILearnerRepository repository) : IRequestHandler<GetCourseCodesByUkprnQuery, GetCourseCodesByUkprnResult>
{
    public async Task<GetCourseCodesByUkprnResult> Handle(GetCourseCodesByUkprnQuery request, CancellationToken cancellationToken)
    {
        var codes = await repository.GetCourseCodesByUkprn(request.Ukprn, cancellationToken);

        return new GetCourseCodesByUkprnResult()
        {
            CourseCodes = codes
        };
    }
}