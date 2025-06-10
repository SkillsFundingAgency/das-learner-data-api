using SFA.DAS.LearnerData.Application.Queries.GetSearch;

namespace SFA.DAS.LearnerData.Api.Models.Responses;

public record GetSearchResponse : PagedResponse<GetSearchResponseItem>
{
    public DateTime? LastSubmissionDate { get; set; }
    
    public static GetSearchResponse MapFrom(GetSearchResult result)
    {
        return new GetSearchResponse
        {
            LastSubmissionDate = result.LastSubmissionDate,
            Data = result.Items.Select(item => new GetSearchResponseItem
            {
                Id = item.Id,
                CreatedDate = item.CreatedDate,
                UpdatedDate = item.UpdatedDate,
                Uln = item.Uln,
                Ukprn = item.Ukprn,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                Dob = item.Dob,
                AcademicYear = item.AcademicYear,
                StartDate = item.StartDate,
                PlannedEndDate = item.PlannedEndDate,
                PercentageLearningToBeDelivered = item.PercentageLearningToBeDelivered,
                EpaoPrice = item.EpaoPrice,
                TrainingPrice = item.TrainingPrice,
                AgreementId = item.AgreementId,
                ConsumerReference = item.ConsumerReference,
                CorrelationId = item.CorrelationId,
                ReceivedDate = item.ReceivedDate,
                StandardCode = item.StandardCode,
                IsFlexiJob = item.IsFlexiJob,
                PlannedOTJTrainingHours = item.PlannedOTJTrainingHours
            }),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }
}

public record GetSearchResponseItem : LearnerResponse
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
    public int StandardCode { get; set; }
    public bool IsFlexiJob { get; set; }
    public int PlannedOTJTrainingHours { get; set; }
    public DateTime ReceivedDate { get; set; }
    public Guid CorrelationId { get; set; }
    public string ConsumerReference { get; set; }
}

