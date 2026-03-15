namespace Infrastructure.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Uri? Icon { get; set; }
    public string Color { get; set; }

    public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
}