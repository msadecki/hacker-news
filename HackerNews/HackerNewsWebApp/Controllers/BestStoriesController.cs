using System.ComponentModel.DataAnnotations;
using System.Net;
using HackerNewsWebApp.Features.BestStories.Dtos;
using HackerNewsWebApp.Features.BestStories.Queries.List;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsWebApp.Controllers;

[ApiController, Route("api/[controller]"), Produces("application/json")]
public sealed class BestStoriesController(ISender sender) : ControllerBase
{
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyCollection<BestStoryDto>>> GetBestStories([Required] int topCount, CancellationToken cancellationToken)
    {
        try
        {
            var bestStories = await sender.Send(new ListBestStoriesQuery(topCount), cancellationToken);

            return Ok(bestStories);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = new { ex.Message } });
        }
    }
}