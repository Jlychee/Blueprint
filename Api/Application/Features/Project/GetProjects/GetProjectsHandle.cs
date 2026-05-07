using Infrastructure.Repositories.Interfaces;
using Client.Models.Models.DTO;
using MediatR;
using Api.Application.Features.Project.GetProjects;

namespace Api.Application.Features.Project.GetProject;

public class GetProjectsHandle(IProjectRepository projectRepository, IMetricRepository metricRepository)
    : IRequestHandler<GetProjectsQuery, PagedResultDto<ProjectCardDto>>
{
    public async Task<PagedResultDto<ProjectCardDto>> Handle(GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var occurredAtUtc = DateTime.UtcNow;
        await metricRepository.RegisterFilteredViewAsync(request.cookie.MetricUserId,
            request.cookie.FilterSessionId,
            request.filter.Page,            
            occurredAtUtc,
            cancellationToken);
        return await projectRepository.SearchAsync(
                request.filter,
                request.cookie.MetricUserId,
                cancellationToken)
            ?? throw new KeyNotFoundException();
    }
}