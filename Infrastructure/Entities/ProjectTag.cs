namespace Infrastructure.Entities;

public class ProjectTag
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }

    public Guid TagId { get; set; }
    public Tag Tag { get; set; }
}