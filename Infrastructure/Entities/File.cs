namespace Infrastructure.Entities;

public class File
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }

    public Uri? CustDev { get; set; }
    public Uri? Description { get; set; }
    public Uri? Mvp { get; set; }
    public Uri? RoadMap { get; set; }
    public Uri? Product { get; set; }
}