namespace Api.Application.Features.Project.PutLike;
using Infrastructure.Repositories.Interfaces;
using MediatR;

public class PutLikeHandler(IProjectRepository projectRepository, IMetricRepository metricRepository)
    : IRequestHandler<PutLikeQuery, bool>
{
    public async Task<bool> Handle(PutLikeQuery request, CancellationToken cancellationToken)
    {
        var status = await projectRepository.LikeProjectAsync(request.Id,request.cookie.FilterSessionId, cancellationToken);
        return status;
    }
}