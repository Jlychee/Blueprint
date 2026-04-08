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
        // System.Console.WriteLine(request.filter.Search);
        // System.Console.WriteLine(request.filter.Page);
        // System.Console.WriteLine(request.filter.PageSize);
        // System.Console.WriteLine(request.filter.Semester);
        // System.Console.WriteLine(request.filter.TagIds);
        // System.Console.WriteLine(request.filter.TeamMemberCount);
        // System.Console.WriteLine(request.filter.Year);

        return await projectRepository.SearchAsync(request.filter, cancellationToken)
            ?? throw new Exception("Get projects exception");


    }
}