using Infrastructure.Repositories.Interfaces;
using Client.Models.Models.DTO;
using MediatR;
using Api.Application.Features.Project.GetProjects;

namespace Api.Application.Features.Project.GetProject;


public class GetProjectsHandle(IProjectRepository projectRepository) : IRequestHandler<GetProjectsQuery, List<ProjectCardDto>>
{
    public async Task<List<ProjectCardDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        System.Console.WriteLine(request.filter.Search);

        return
        [
            new()
            {
                Id = 1,
                Name = "ABOBA",
                ShortDescriptionAi = "ABOBA AI",
                Tags = []
            }
        ];
    }
}