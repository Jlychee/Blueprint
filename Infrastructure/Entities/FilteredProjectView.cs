namespace Infrastructure.Entities;

public class FilteredProjectView
{
    public Guid Id { get; set; }
    public Guid FilterSessionId { get; set; }
    public int ProjectId { get; set; }
    public DateTime OpenedAtUtc { get; set; }
}
