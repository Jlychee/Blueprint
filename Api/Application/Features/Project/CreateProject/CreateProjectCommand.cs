using MediatR;

namespace Api.Application.Features.Project.CreateProject;

public record CreateProjectModel(string Name, string? Semester, int? Year) : IRequest<Guid>;