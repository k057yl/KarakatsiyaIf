using Karakatsiya.Constants;
using Karakatsiya.Features.Events.Queries.GetAddressByCoords;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Karakatsiya.Controllers
{
    [ApiController]
    [Route("api/geo")]
    public class GeoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GeoController(IMediator _mediator)
        {
            this._mediator = _mediator;
        }

        [HttpGet("reverse")]
        [EnableRateLimiting(AppConstants.Shared.GEO_RATE_LIMITER_POLICY)]
        public async Task<IActionResult> ReverseGeocode([FromQuery] double lat, [FromQuery] double lon, CancellationToken cancellationToken)
        {
            var query = new GetAddressByCoordsQuery(lat, lon);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
