using Client.Models.Models.DTO;
using Infrastructure.Repositories.Interfaces;
using MediatR;

namespace Api.Application.Features.Project.GetProject;

public class GetProjectHandle(IProjectRepository projectRepository, IMetricRepository metricRepository)
    : IRequestHandler<GetProjectQuery, FullProjectInfo>
{
    public async Task<FullProjectInfo?> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetFullProjectInfoAsync(request.Id) ??
                      throw new KeyNotFoundException($"{request.Id}");
        if (project is null)
            return null;

        var occurredAtUtc = DateTime.UtcNow;
        await metricRepository.RegisterOpenAsync(
            request.cookie.MetricUserId, 
            DateOnly.FromDateTime(occurredAtUtc),
            cancellationToken);
        
        await metricRepository.RegisterFilteredProjectViewAsync(
            request.cookie.MetricUserId,
            request.cookie.FilterSessionId,
            request.Id, request.cookie.FilterSessionId != Guid.Empty, occurredAtUtc,
            cancellationToken);
        return project;
    }
}