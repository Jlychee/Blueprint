using Infrastructure.Repositories.Interfaces;
using Client.Models.Models.DTO;
using MediatR;
using Api.Application.Features.Project.GetProjects;

namespace Api.Application.Features.Project.GetProject;

public class GetProjectsHandle(IProjectRepository projectRepository, ITagRepository tagRepository)
    : IRequestHandler<GetProjectsQuery, PagedResultDto<ProjectCardDto>>
{
    public async Task<PagedResultDto<ProjectCardDto>> Handle(GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = request.filter;
        if (filter.TagNames is not null)
            filter.TagIds = await tagRepository.GetTagsIdsByNameAsync(filter.TagNames, cancellationToken);

        return await projectRepository.SearchAsync(request.filter, cancellationToken)
            ?? throw new KeyNotFoundException();
    }
}