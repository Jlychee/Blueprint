using Client.Models.Models.DTO;
using MediatR;


namespace Api.Application.Features.Project.GetProjects;

public record GetProjectsQuery(ProjectCatalogFilter filter): IRequest<List<ProjectCardDto>>;