namespace SFA.DAS.LearnerData.Application.Queries.GetSearch;

public class GetLearnersByIdRequest
{
    public List<long> LearnerIds { get; set; } = new();
}