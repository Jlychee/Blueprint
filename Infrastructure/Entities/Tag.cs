namespace Infrastructure.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Title { get; set; }
    public Uri? Icon { get; set; }
    public string? Color { get; set; }
    
    public int TagTypeId { get; set; }
    public TagType TagType { get; set; }

    public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
}