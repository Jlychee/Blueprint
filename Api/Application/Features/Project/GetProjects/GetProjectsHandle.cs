using Infrastructure.Repositories.Interfaces;
using Client.Models.Models.DTO;
using MediatR;
using Api.Application.Features.Project.GetProjects;

namespace Api.Application.Features.Project.GetProject;

public class GetProjectsHandle(IProjectRepository projectRepository)
    : IRequestHandler<GetProjectsQuery, PagedResultDto<ProjectCardDto>>
{
    public async Task<PagedResultDto<ProjectCardDto>> Handle(GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        return await projectRepository.SearchAsync(request.filter, cancellationToken)
            ?? throw new KeyNotFoundException();
    }
}