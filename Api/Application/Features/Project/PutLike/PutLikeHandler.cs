namespace Api.Application.Features.Project.PutLike;
using Infrastructure.Repositories.Interfaces;
using MediatR;

public class PutLikeHandler(IProjectRepository projectRepository)
    : IRequestHandler<PutLikeQuery, bool>
{
    public async Task<bool> Handle(PutLikeQuery request, CancellationToken cancellationToken)
    {
        var likedAtUtc = DateTime.UtcNow;
        var status = await projectRepository.LikeProjectAsync(
            request.Id,
            request.cookie.MetricUserId,
            likedAtUtc,
            cancellationToken);
        return status;
    }
}
