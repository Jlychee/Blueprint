namespace Infrastructure.Repositories.Interfaces;

public interface IMetricRepository
{
    Task RegisterOpenAsync(Guid userId, DateOnly occurredAt, CancellationToken ct);
    Task RegisterFilteredProjectViewAsync(Guid filterSessionId, int projectId, DateTime occurredAtUtc, CancellationToken ct);
    Task RebuildOpenCohortsRetentionAsync(CancellationToken ct);
}
