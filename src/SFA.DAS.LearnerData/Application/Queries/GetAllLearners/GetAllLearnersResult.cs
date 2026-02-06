using SFA.DAS.LearnerData.Messages;

namespace SFA.DAS.LearnerData.Application.Queries.GetAllLearners;

public record GetAllLearnersResult : PagedQueryResult<GetAllLearnersResultItem>;

public record GetAllLearnersResultItem : LearnerResult
{
    public long Id { get; set; }
    public long Uln { get; set; }
    public long Ukprn { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Email { get; set; }
    public DateTime Dob { get; set; }
    public int AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int? PercentageLearningToBeDelivered { get; set; }
    public int EpaoPrice { get; set; }
    public int TrainingPrice { get; set; }
    public string? AgreementId { get; set; }
    public string? TrainingCode { get; set; }
    public string? TrainingName { get; set; }
    public LearningType? LearningType { get; set; }
    public bool IsFlexiJob { get; set; }
    public int PlannedOTJTrainingHours { get; set; }
    public DateTime ReceivedDate { get; set; }
    public Guid CorrelationId { get; set; }
    public string ConsumerReference { get; set; }
}
