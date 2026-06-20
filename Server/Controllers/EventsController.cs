using Karakatsiya.Constants;
using Karakatsiya.Data.Enums;
using Karakatsiya.Features.Events.Commands.CreateEvent;
using Karakatsiya.Features.Events.Commands.DeleteEvent;
using Karakatsiya.Features.Events.Commands.UpdateEvent;
using Karakatsiya.Features.Events.Commands.UploadOrganizerPhoto;
using Karakatsiya.Features.Events.Queries.GetApprovedEvents;
using Karakatsiya.Features.Events.Queries.GetArchivedEvents;
using Karakatsiya.Features.Events.Queries.GetEventDetails;
using Karakatsiya.Features.Events.Queries.GetOccupiedDates;
using Karakatsiya.Features.Events.Queries.GetOrganizerEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Karakatsiya.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Organizer))]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });

            var eventId = await Mediator.Send(command with { UserId = userId });
            return Ok(new { Message = AppConstants.Success.EVENT_CREATED, EventId = eventId });
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovedEvents() => Ok(await Mediator.Send(new GetApprovedEventsQuery()));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEventDetails(Guid id) => Ok(await Mediator.Send(new GetEventDetailsQuery(id)));

        [HttpGet("archive")]
        public async Task<IActionResult> GetArchivedEvents() => Ok(await Mediator.Send(new GetArchivedEventsQuery()));

        [HttpGet("my")]
        [Authorize(Roles = nameof(UserRole.Organizer))]
        public async Task<IActionResult> GetOrganizerEvents()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });

            return Ok(await Mediator.Send(new GetOrganizerEventsQuery(userId)));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Organizer))]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventCommand command)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });

            var success = await Mediator.Send(command with { EventId = id, OrganizerId = userId });
            if (!success) return NotFound(new { Message = AppConstants.Errors.PERFORMER_NOT_FOUND });

            return Ok(new { Message = AppConstants.Success.CONTACTS_UPDATED });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Organizer))]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });

            await Mediator.Send(new DeleteEventCommand(id));
            return Ok(new { Message = AppConstants.Success.EVENT_DELETED });
        }

        [HttpPost("{id:guid}/photos/organizer")]
        [Authorize(Roles = nameof(UserRole.Organizer))]
        public async Task<IActionResult> UploadOrganizerPhoto(Guid id, [FromForm] IFormFile file, [FromForm] bool isMain)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });

            var result = await Mediator.Send(new UploadOrganizerPhotoCommand(id, userId, file, isMain));
            if (!result.Success) return BadRequest(new { Message = result.ErrorMessage ?? AppConstants.Errors.VALIDATION_FAILED });

            return Ok(new { result.Url, Message = AppConstants.Success.AVATAR_UPLOADED });
        }

        [HttpGet("calendar/occupied-dates")]
        public async Task<IActionResult> GetOccupiedDates([FromQuery] int year, [FromQuery] int month)
            => Ok(await Mediator.Send(new GetOccupiedDatesQuery(year, month)));

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}