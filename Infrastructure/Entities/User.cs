namespace Infrastructure.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}