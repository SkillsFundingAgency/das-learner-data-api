using SFA.DAS.LearnerData.Application.Queries.GetAllLearners;

namespace SFA.DAS.LearnerData.Api.Models.Responses;

public class GetAllLearnersResponse
{
    public IEnumerable<GetAllLearnersResponseItem> Data { get; set; } = [];
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }

    public static GetAllLearnersResponse MapFrom(GetAllLearnersResult result)
    {
        return new GetAllLearnersResponse
        {
            Data = result.Items.Select(item => new GetAllLearnersResponseItem
            {
                Id = item.Id,
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
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            PageSize = result.PageSize,
            Page = result.Page
        };
    }
}

public class GetAllLearnersResponseItem
{
    public long Id { get; set; }
    public long Uln { get; set; }
    public long Ukprn { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime Dob { get; set; }
    public int AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int? PercentageLearningToBeDelivered { get; set; }
    public int EpaoPrice { get; set; }
    public int TrainingPrice { get; set; }
    public string? AgreementId { get; set; }
    public string ConsumerReference { get; set; } = string.Empty;
    public Guid CorrelationId { get; set; }
    public DateTime ReceivedDate { get; set; }
    public int StandardCode { get; set; }
    public bool IsFlexiJob { get; set; }
    public int PlannedOTJTrainingHours { get; set; }
}
