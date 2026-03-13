namespace Infrastructure.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DescriptionAi { get; set; }
    public string ShortDescriptionAi { get; set; }
    public int Year { get; set; }
    public int Semester { get; set; }

    public ICollection<TeamMember> TeamMembers = new List<TeamMember>();
}