using Client.Models.Models.DTO;

namespace Infrastructure.Entities;

public class FilteredView
{
    public int Id { get; set; }
    public Guid UserId  { get; set; }
    public Guid FilterSessionId { get; set; }
    public int Page { get; set; }
    public ProjectCatalogFilter? Filter { get; set; }
    public DateTime OpenedAtUtc { get; set; }
}