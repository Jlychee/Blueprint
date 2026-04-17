namespace Infrastructure.Entities;

public class UserRetentionState
{
    public Guid UserId { get; set; }
    public DateOnly FirstOpen { get; set; }
    public DateOnly SecondOpen { get; set; }
    public Boolean r7D { get; set;}
    public Boolean r14D { get; set;}
    public Boolean r30D { get; set;}
}