namespace Infrastructure;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<TeamMember> TeamMembers = new List<TeamMember>();
}