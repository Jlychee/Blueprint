namespace Client.Models.Models.Entities;

public class FullProjectInfo(
    string name,
    string descriptionAi,
    List<File> files,
    int? year,
    int? semester,
    ICollection<Tag> tags,
    ICollection<TeamMember> teamMembers)
{
    public required string Name { get; set; } = name;
    public required string DescriptionAi { get; set; } = descriptionAi;
    public List<File> Files { get; set; } = files;
    public int? Year { get; set; } = year;
    public int? Semester { get; set; } = semester;
    public ICollection<Tag> Tags { get; set; } = tags;
    public ICollection<TeamMember> TeamMembers { get; set; } = teamMembers;
}