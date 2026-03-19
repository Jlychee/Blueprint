using Api.Application.Features.Project.CreateProject;
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
    
}