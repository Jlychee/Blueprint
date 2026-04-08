using MediatR;

namespace Api.Application.Features.Project.GetProject;

public record GetProjectQuery(Guid Id) : IRequest<GetProjectResponse>;