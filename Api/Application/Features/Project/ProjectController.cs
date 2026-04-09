using Api.Application.Features.Project.CreateProject;
using Api.Application.Features.Project.GetProject;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        if (!Guid.TryParse(Request.Cookies["metric_user_id"],out var metricId))
            metricId = Guid.Empty;

        if (!Guid.TryParse(Request.Cookies["filter_session_id"],out var filterSessionId))
            filterSessionId = Guid.Empty;
        
        var result = await mediator.Send(new GetProjectQuery(id,metricId,filterSessionId));
        return Ok(result);
    }
}
