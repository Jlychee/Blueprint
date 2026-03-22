
using Client.Models.Models.Enums;
using Infrastructure.Entities;

public class TagType
{
    public int Id { get; set; }
    public string Name { get; set; }
    // TODO: мммм че-то, не нравится мне это
    public TagCategory Type { get; set; }
    public int Priority { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}