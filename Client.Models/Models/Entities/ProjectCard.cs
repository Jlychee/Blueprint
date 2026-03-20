namespace Client.Models.Models.Entities;

public class ProjectCard(string name, string shortDescriptionAi, int? year, int? semester, ICollection<Tag>? tags)
{
    public required string Name { get; set; } = name;
    public required string ShortDescriptionAi { get; set; } = shortDescriptionAi;
    public int? Year { get; set; } = year;
    public int? Semester { get; set; } = semester;
    public ICollection<Tag>? Tags { get; set; } = tags;
}