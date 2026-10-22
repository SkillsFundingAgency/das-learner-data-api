using SFA.DAS.LearnerData.Application.Queries.GetLearnerById;

namespace SFA.DAS.LearnerData.Api.Models.Responses;

public static class GetLearnersByIdResponseMapper
{
    public static List<GetLearnerByIdResponse> MapFrom(IEnumerable<GetLearnerByIdResult> results)
    {
        return results.Select(GetLearnerByIdResponse.MapFrom).ToList();
    }
}
