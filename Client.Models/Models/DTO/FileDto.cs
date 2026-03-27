namespace Client.Models.Models.DTO;

public class FileDto
{
    public Uri? CustDev { get; set; }
    public Uri? Description { get; set; }
    public Uri? Mvp { get; set; }
    public Uri? RoadMap { get; set; }
    public List<Uri>? Product { get; set; }
}