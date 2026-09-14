namespace SFA.DAS.LearnerData.Application.Queries.GetSearch;

public class GetLearnersByIdRequest
{
    public List<long> Ids { get; set; } = new();
}