using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class MetricRepository: IMetricRepository
{
    public Task RegisterOpenAsync(Guid userId, DateOnly occurredAt, CancellationToken ct)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Task RebuildOpenCohortsRetentionAsync(CancellationToken ct)
    {
        // TODO
        throw new NotImplementedException();
    }
}