namespace SFA.DAS.LearnerData.Api.Models.Requests;

public class GetLearnersByIdRequest
{
    public List<long> LearnerIds { get; set; } = new();
}
