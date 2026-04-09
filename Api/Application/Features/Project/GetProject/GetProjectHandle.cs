using Client.Models.Models.DTO;
using Infrastructure.Repositories.Interfaces;
using MediatR;

namespace Api.Application.Features.Project.GetProject;

public class GetProjectHandle(IProjectRepository projectRepository,IMetricRepository metricRepository): IRequestHandler<GetProjectQuery, FullProjectInfo>
{
    public async Task<FullProjectInfo?> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        metricRepository.RegisterOpenAsync(request.MetricId, DateOnly.FromDateTime(DateTime.UtcNow),cancellationToken);
        return await projectRepository.GetFullProjectInfoAsync(request.Id);
    }
}