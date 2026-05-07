namespace Infrastructure.Entities;

public class Like
{
    public int ProjectId { get; set; }
    public Guid UserId { get; set; }
    public DateTime LikedAtUtc { get; set; }

    public Project Project { get; set; } = null!;
}
