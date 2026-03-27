using Api.Application.Features.Project.CreateProject;
using Api.Application.Features.Project.GetProject;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Features.Project;

[ApiController]
[Route("api/project/load_table")]
public class TableController(IMediator mediator) : ControllerBase
{
    [HttpPost("api/[controller")]
    public async Task<IActionResult> LoadTable([FromBody] LoadTableCommand command)
    {
        
    }
}