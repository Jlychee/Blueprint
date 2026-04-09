using Client.Models.Models.DTO;
using MediatR;

namespace Api.Application.Features.Project.GetProject;

public record GetProjectQuery(int Id) : IRequest<FullProjectInfo>;