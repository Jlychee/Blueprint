using Api.Application.Features.Project.GetProject;
using Api.Application.Features.Project.GetProjects;
using Api.Application.Features.Project.GetTags;
using Api.Application.Features.Project.DeleteLike;
using Api.Application.Features.Project.PutLike;
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
    public async Task<IActionResult> GetProject(int id,[FromUserCookie] UserCookie cookie)
    {
        var result = await mediator.Send(new GetProjectQuery(id,cookie));
    
        if (result is null)
            return NotFound();
            
        return Ok(result);
    }
    [HttpPut("project/{id:int}/like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutLike(int id,[FromUserCookie] UserCookie cookie)
    {
        var result = await mediator.Send(new PutLikeQuery(id,cookie));
    
        if (result is false)
            return NotFound();
            
        return Ok(result);
    }
    [HttpDelete("project/{id:int}/like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLike(int id,[FromUserCookie] UserCookie cookie)
    {
        var result = await mediator.Send(new DeleteLikeQuery(id,cookie));

        if (result is false)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("projects")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjects([FromQuery] ProjectCatalogFilter filter,[FromUserCookie] UserCookie cookie)
    {
        var result = await mediator.Send(new GetProjectsQuery(filter, cookie));
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
