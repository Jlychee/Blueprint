using Api.Application.Features.Metric.RebuildOpenCohortsRetention;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Features.Metric;

[ApiController]
[Route("api/metrics")]
public class MetricController(IMediator mediator) : ControllerBase
{
    [HttpHead("rebuild_open_cohorts_retention")]
    public async Task<IActionResult> RebuildOpenCohortsRetention()
    {
        await mediator.Send(new RebuildOpenCohortsRetentionCommand());
        return Ok();
    }
}
