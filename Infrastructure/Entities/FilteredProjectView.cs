namespace Infrastructure.Entities;

public class FilteredProjectView
{
    public int Id { get; set; }
    public Guid UserId  { get; set; }
    public Guid FilterSessionId { get; set; }
    public int ProjectId { get; set; }
    public bool HasFilter { get; set; }
    public DateTime OpenedAtUtc { get; set; }
}
