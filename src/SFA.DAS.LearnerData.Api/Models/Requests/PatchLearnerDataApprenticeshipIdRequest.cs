namespace SFA.DAS.LearnerData.Api.Models.Requests;

public record PatchLearnerDataApprenticeshipIdRequest
{
    public long ApprenticeshipId { get; set; }
}