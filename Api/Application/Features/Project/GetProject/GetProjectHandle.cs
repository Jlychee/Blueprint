using MediatR;

namespace Api.Application.Features.Project.GetProject;

public class GetProjectHandle: IRequestHandler<GetProjectQuery, GetProjectResponse>
{
    public Task<GetProjectResponse> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetProjectResponse("Test", "весенний", 2026)); //заглушка
    }
}