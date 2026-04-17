namespace Infrastructure.Repositories.Interfaces;

public interface IMetricRepository
{
    Task RegisterOpenAsync(Guid userId, DateOnly occurredAt, CancellationToken ct);
    Task RegisterFilteredProjectViewAsync(Guid userId, Guid filterSessionId, int projectId,bool hasFilter, DateTime occurredAtUtc, CancellationToken ct);
    Task RebuildOpenCohortsRetentionAsync(CancellationToken ct);
    Task RegisterFilteredViewAsync(Guid userId, Guid filterSessionId,int page, DateTime occurredAtUtc, CancellationToken ct);
}
