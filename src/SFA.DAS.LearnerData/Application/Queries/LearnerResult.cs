namespace SFA.DAS.LearnerData.Application.Queries;

public abstract record LearnerResult
{
    public long Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}