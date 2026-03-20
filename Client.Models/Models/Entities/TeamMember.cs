using Client.Models.Models.Enums;

namespace Client.Models.Models.Entities;

public class TeamMember(User user, TeamRole role)
{
    public User User { get; set; } = user;
    public TeamRole Role { get; set; } = role;
}