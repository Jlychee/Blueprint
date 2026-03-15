using Client.Models.Models.Enums;

namespace Infrastructure.Entities;

public class TeamMember
{
    public TeamRole Role { get; set; }

    public Project Project { get; set; }
    public Guid ProjectId { get; set; }

    public User User { get; set; }
    public Guid UserId { get; set; }
}