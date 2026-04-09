using Client.Models.Models.DTO;
using Infrastructure.Repositories.Interfaces;
using MediatR;

namespace Api.Application.Features.Project.GetProject;

public class GetProjectHandle(IProjectRepository projectRepository,IMetricRepository metricRepository): IRequestHandler<GetProjectQuery, FullProjectInfo>
{
    public async Task<FullProjectInfo?> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetFullProjectInfoAsync(request.Id);
        if (project is null)
            return null;

        var occurredAtUtc = DateTime.UtcNow;
        await metricRepository.RegisterOpenAsync(request.MetricId, DateOnly.FromDateTime(occurredAtUtc), cancellationToken);
        await metricRepository.RegisterFilteredProjectViewAsync(request.FilterSessionId, request.Id, occurredAtUtc, cancellationToken);
        return project;
    }
}
