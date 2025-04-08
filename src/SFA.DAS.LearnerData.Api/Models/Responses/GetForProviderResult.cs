using SFA.DAS.LearnerData.Application.Queries.GetAll;

namespace SFA.DAS.LearnerData.Api.Models.Responses;

public record GetForProviderResponse
{
    public IEnumerable<GetForProviderResponseItem> Learners { get; set; }
}

public record GetForProviderResponseItem
{
    public long Id { get; set; }
    public long Uln { get; set; }
    public long Ukprn { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime Dob { get; set; }
    public int AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int PercentageLearningToBeDelivered { get; set; }
    public int EpaoPrice { get; set; }
    public int TrainingPrice { get; set; }
    public string AgreementId { get; set; }
    public int StandardCode { get; set; }
    public bool IsFlexiJob { get; set; }
    public int PlannedOTJTrainingHours { get; set; }
    public DateTime ReceivedDate { get; set; }
    public string CorrelationId { get; set; }
    public string ConsumerReference { get; set; }
}