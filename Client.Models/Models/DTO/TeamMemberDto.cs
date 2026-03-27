using Client.Models.Models.Enums;

namespace Client.Models.Models.DTO;

public class TeamMemberDto
{
    public string UserName { get; set; }
    public TeamRole? Role { get; set; }
}