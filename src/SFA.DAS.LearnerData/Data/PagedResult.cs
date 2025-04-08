namespace SFA.DAS.LearnerData.Data;

public record PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; } = 1;
}