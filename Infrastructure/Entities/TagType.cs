using Infrastructure.Entities;

public class TagType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Priority { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}