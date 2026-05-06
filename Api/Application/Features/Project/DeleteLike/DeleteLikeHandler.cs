namespace Api.Application.Features.Project.PutLike;
using Infrastructure.Repositories.Interfaces;
using MediatR;

public class DeleteLikeHandler(IProjectRepository projectRepository, IMetricRepository metricRepository)
    : IRequestHandler<DeleteLikeQuery, bool>
{
    public async Task<bool> Handle(DeleteLikeQuery request, CancellationToken cancellationToken)
    {
        var status = await projectRepository.UnlikeProjectAsync(request.Id,request.cookie.FilterSessionId, cancellationToken);
        return status;
    }
}