namespace Client.Models.Models.DTO;

public sealed class ProjectCatalogFilter
{
    public string? Search { get; init; }
    public List<int>? TagIds { get; init; }
    public int TeamMemberCount { get; init; }
    public int? Year { get; init; }
    public int? Semester { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; } = 16;
}