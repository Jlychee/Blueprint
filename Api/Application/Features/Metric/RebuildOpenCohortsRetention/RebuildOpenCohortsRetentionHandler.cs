using Infrastructure.Repositories.Interfaces;
using MediatR;

namespace Api.Application.Features.Metric.RebuildOpenCohortsRetention;

public class RebuildOpenCohortsRetentionHandler(IMetricRepository metricRepository) : IRequestHandler<RebuildOpenCohortsRetentionCommand>
{
    public async Task Handle(RebuildOpenCohortsRetentionCommand request, CancellationToken cancellationToken)
    {
        await metricRepository.RebuildOpenCohortsRetentionAsync(cancellationToken);
    }
}
