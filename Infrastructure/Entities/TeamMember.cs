using Client.Models.Models.Enums;

namespace Infrastructure.Entities;

public class TeamMember
{
    public TeamRole Role { get; set; }

    public Project Project { get; set; }
    public int ProjectId { get; set; }

    public User User { get; set; }
    public int UserId { get; set; }
}