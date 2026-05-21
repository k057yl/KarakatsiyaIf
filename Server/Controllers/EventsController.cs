using Karakatsiya.Constants;
using Karakatsiya.Features.Events.Commands.CreateEvent;
using Karakatsiya.Features.Events.Queries.GetApprovedEvents;
using Karakatsiya.Features.Events.Queries.GetArchivedEvents;
using Karakatsiya.Features.Events.Queries.GetEventDetails;
using Karakatsiya.Models.Dtos.Event;
using Karakatsiya.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Karakatsiya.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Organizer))]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto payload)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });
            }

            var command = new CreateEventCommand(userId, payload);
            var eventId = await Mediator.Send(command);

            return Ok(new
            {
                Message = AppConstants.Success.EVENT_CREATED,
                EventId = eventId
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovedEvents()
        {
            var events = await Mediator.Send(new GetApprovedEventsQuery());
            return Ok(events);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEventDetails(Guid id)
        {
            var query = new GetEventDetailsQuery(id);
            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("archive")]
        public async Task<IActionResult> GetArchivedEvents()
        {
            var events = await Mediator.Send(new GetArchivedEventsQuery());
            return Ok(events);
        }
    }
}
