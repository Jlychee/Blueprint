namespace Api.Application.Features.Project.GetProject;

public record GetProjectResponse(string Name, string? Semester, int? Year);