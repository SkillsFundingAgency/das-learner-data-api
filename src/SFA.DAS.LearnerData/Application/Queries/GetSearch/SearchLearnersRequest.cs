namespace SFA.DAS.LearnerData.Application.Queries.GetSearch;

public class SearchLearnersRequest
{
    public int? StartMonth { get; set; }
    public int StartYear { get; set; }
    public int Page { get; set; } = 1;
    public int? PageSize { get; set; } = 20;
    public string? SortColumn { get; set; } = string.Empty;
    public bool SortDescending { get; set; } = false;
    public string? Filter { get; set; } = string.Empty;
    public bool ExcludeApproved { get; set; } = true;
    public string? MaxStartDate { get; set; } = string.Empty;
    public string? ExcludeUlns { get; set; } = string.Empty;
    public int? CourseCode { get; set; } = null;
    public string? LearningType { get; set; } = string.Empty;
}