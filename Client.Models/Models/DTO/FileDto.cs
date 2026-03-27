namespace Client.Models.Models.DTO;

public class FileDto
{
    public Uri? CustDev { get; set; }
    public Uri? Description { get; set; }
    
    public List<Uri>? Mvp { get; set; }
    public Uri? RoadMap { get; set; }
    public Uri? Product { get; set; }
}