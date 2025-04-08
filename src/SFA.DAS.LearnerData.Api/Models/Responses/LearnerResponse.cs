namespace SFA.DAS.LearnerData.Api.Models.Responses;

public abstract record LearnerResponse
{
    public long Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}