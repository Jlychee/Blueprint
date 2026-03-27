namespace Client.Models.Models.DTO;

public class FullProjectInfo
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public int Year { get; set; }
    public int Semester { get; set; }

    public FileDto Files { get; set; }
    public List<TeamMemberDto> TeamMembers { get; set; } = [];
    public List<TagDto> Tags { get; set; } = [];
}