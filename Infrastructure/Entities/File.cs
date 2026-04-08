namespace Infrastructure.Entities;

public class File
{
    public int ProjectId { get; set; }
    public Project Project { get; set; }

    public Uri? CustDev { get; set; }
    public Uri? Description { get; set; }
    public Uri? Mvp { get; set; }
    public Uri? RoadMap { get; set; }
    public List<Uri>? Product { get; set; }
}