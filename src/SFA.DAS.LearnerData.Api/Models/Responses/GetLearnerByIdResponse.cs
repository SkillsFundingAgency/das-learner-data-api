using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.LearnerData.Application.Queries.GetLearnerById;
using SFA.DAS.LearnerData.Extensions;
using SFA.DAS.LearnerData.Messages;

namespace SFA.DAS.LearnerData.Api.Models.Responses;

public record GetLearnerByIdResponse : LearnerResponse
{
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

    [JsonConverter(typeof(StringEnumConverter))]
    public LearningType? LearningType { get; set; }
    public bool IsFlexiJob { get; set; }
    public int PlannedOTJTrainingHours { get; set; }
    public DateTime ReceivedDate { get; set; }
    public Guid CorrelationId { get; set; }
    public string ConsumerReference { get; set; }
    public long? ApprenticeshipId { get; set; }

    public static GetLearnerByIdResponse MapFrom(GetLearnerByIdResult result)
    {
        return new GetLearnerByIdResponse
        {
            Id = result.Id,
            CreatedDate = result.CreatedDate,
            UpdatedDate = result.UpdatedDate,
            Uln = result.Uln,
            Ukprn = result.Ukprn,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Email = result.Email,
            Dob = result.Dob,
            AcademicYear = result.AcademicYear,
            StartDate = result.StartDate,
            PlannedEndDate = result.PlannedEndDate,
            PercentageLearningToBeDelivered = result.PercentageLearningToBeDelivered,
            EpaoPrice = result.EpaoPrice,
            TrainingPrice = result.TrainingPrice,
            AgreementId = result.AgreementId,
            ConsumerReference = result.ConsumerReference,
            CorrelationId = result.CorrelationId,
            ReceivedDate = result.ReceivedDate,
            TrainingCode = result.TrainingCode,
            TrainingName = result.TrainingName,
            LearningType = result.LearningType,
            IsFlexiJob = result.IsFlexiJob,
            PlannedOTJTrainingHours = result.PlannedOTJTrainingHours,
            ApprenticeshipId = result.ApprenticeshipId,
        };
    }
}