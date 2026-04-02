using Api.Application.Features.Table.LoadTable;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Features.Table;

[ApiController]
[Route("api/project")]
public class TableController(IMediator mediator) : ControllerBase
{
    [HttpPost("load_table")]
    public async Task<IActionResult> LoadTable([FromForm] LoadTableCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}