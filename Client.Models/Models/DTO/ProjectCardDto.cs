namespace Client.Models.Models.DTO;

public class ProjectCardDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortDescriptionAi { get; set; }
    public List<TagDto> Tags { get; set; } = [];

    public int LikeCount { get; set; }
    public bool IsLiked { get; set; }
}
