namespace Api.Application.Features.Project.DeleteLike;
using Infrastructure.Repositories.Interfaces;
using MediatR;

public class DeleteLikeHandler(IProjectRepository projectRepository)
    : IRequestHandler<DeleteLikeQuery, bool>
{
    public async Task<bool> Handle(DeleteLikeQuery request, CancellationToken cancellationToken)
    {
        var status = await projectRepository.UnlikeProjectAsync(request.Id,request.cookie.MetricUserId, cancellationToken);
        return status;
    }
}
