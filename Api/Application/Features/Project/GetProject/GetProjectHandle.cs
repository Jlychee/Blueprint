using Client.Models.Models.DTO;
using Infrastructure.Repositories.Interfaces;
using MediatR;

namespace Api.Application.Features.Project.GetProject;

public class GetProjectHandle(IProjectRepository projectRepository) : IRequestHandler<GetProjectQuery, FullProjectInfo>
{
    public async Task<FullProjectInfo?> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetFullProjectInfoAsync(request.Id)
            ?? throw new Exception($"{request.Id}");

        return project;
    }
}