using MediatR;
namespace Api.Application.Features.Project.CreateProject;

public class CreateProjectHandler : IRequestHandler<CreateProjectModel, Guid>
{
    public Task<Guid> Handle(CreateProjectModel request, CancellationToken cancellationToken)
    {
        var projectId = Guid.NewGuid();
        return Task.FromResult(projectId);
    }
}