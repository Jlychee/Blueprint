namespace Infrastructure.Repositories.Interfaces;

public interface IMetricRepository
{
    Task RegisterOpenAsync(Guid userId, DateOnly occurredAt, CancellationToken ct);
    Task RebuildOpenCohortsRetentionAsync(CancellationToken ct);
}