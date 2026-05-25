using Karakatsiya.Constants;
using Karakatsiya.Features.Events.Commands.CreateEvent;
using Karakatsiya.Features.Events.Commands.UpdateEvent;
using Karakatsiya.Features.Events.Commands.UploadOrganizerPhoto;
using Karakatsiya.Features.Events.Queries.GetApprovedEvents;
using Karakatsiya.Features.Events.Queries.GetArchivedEvents;
using Karakatsiya.Features.Events.Queries.GetEventDetails;
using Karakatsiya.Features.Events.Queries.GetOrganizerEvents;
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

        [HttpGet("my")]
        [Authorize(Roles = nameof(UserRole.Organizer))]
        public async Task<IActionResult> GetOrganizerEvents()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });
            }

            var events = await Mediator.Send(new GetOrganizerEventsQuery(userId));
            return Ok(events);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Organizer))]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] CreateEventDto payload)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });
            }

            var command = new UpdateEventCommand(id, userId, payload);
            var success = await Mediator.Send(command);

            if (!success)
            {
                return NotFound(new { Message = AppConstants.Errors.VALIDATION_FAILED });
            }

            return Ok(new { Message = AppConstants.Success.EVENT_CREATED });
        }

        [HttpPost("{id:guid}/photos/organizer")]
        [Authorize(Roles = nameof(UserRole.Organizer))]
        public async Task<IActionResult> UploadOrganizerPhoto(Guid id, [FromForm] IFormFile file, [FromForm] bool isMain)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });
            }

            var command = new UploadOrganizerPhotoCommand(id, userId, file, isMain);
            var result = await Mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { Message = result.ErrorMessage });
            }

            return Ok(new { Url = result.Url });
        }
    }
}