namespace SFA.DAS.LearnerData.Api.Models.Responses;

public abstract record PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; } = 1;
}