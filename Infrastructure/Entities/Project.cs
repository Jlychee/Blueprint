namespace Infrastructure.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DescriptionAi { get; set; }
    public string ShortDescriptionAi { get; set; }
    public int Year { get; set; }
    public int Semester { get; set; }
    public File File { get; set; }
    public int LikesCount { get; set; }

    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}