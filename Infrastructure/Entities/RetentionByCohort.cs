namespace Infrastructure.Entities;

public class RetentionByCohort
{
    public DateOnly CohortDate { get; set; }
    public DateOnly CohortWeek { get; set; }
    public int Users { get; set; }
    public int r7D { get; set; }
    public int r14D { get; set; }
    public int r30D { get; set; }
}