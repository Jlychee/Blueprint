using Api.Application.Features.Project.CreateProject;
using Api.Application.Features.Project.GetProject;
using Api.Application.Features.Project.GetProjects;
using Api.Application.Features.Project.GetTags;
using Client.Models.Models.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Features.Project;

[ApiController]
[Route("api/projects")]
public class ProjectController(IMediator mediator) : ControllerBase
{

    [HttpGet("project/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(int id)
    {

        var result = await mediator.Send(new GetProjectQuery(id));
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("projects")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjects([FromQuery] ProjectCatalogFilter filter)
    {
        var result = await mediator.Send(new GetProjectsQuery(filter));
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTags()
    {
        var result = await mediator.Send(new GetTagsQuery());
        System.Console.WriteLine(result.Count);
        if (result is null)
            return NotFound();
        return Ok(result);
    }
}