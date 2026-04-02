using Api.Application.Features.Project.CreateProject;
using Api.Application.Features.Project.GetProject;
using Api.Application.Features.Project.GetProjects;
using Client.Models.Models.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Features.Project;

[ApiController]
[Route("api/projects")]
public class ProjectController(IMediator mediator) : ControllerBase
{
    [HttpPost("create_project")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectModel model)
    {
        var result = await mediator.Send(model);
        return Ok(result);
    }

    [HttpGet("project/{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var result = await mediator.Send(new GetProjectQuery(id));
        return Ok(result);
    }

    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects([FromQuery] ProjectCatalogFilter filter)
    {
        var result = await mediator.Send(new GetProjectsQuery(filter));
        return Ok(result);
    }
}