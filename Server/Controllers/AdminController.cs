using Karakatsiya.Constants;
using Karakatsiya.Features.Admin.Commands.ApproveOrganizer;
using Karakatsiya.Features.Admin.Commands.RejectOrganizer;
using Karakatsiya.Features.Admin.Queries.GetPendingOrganizers;
using Karakatsiya.Features.Events.Commands.ApproveEvent;
using Karakatsiya.Features.Events.Commands.RejectEvent;
using Karakatsiya.Features.Events.Queries.GetPendingEvents;
using Karakatsiya.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karakatsiya.Controllers
{
    [Authorize(Roles = nameof(UserRole.SuperAdmin))]
    [Route("api/[controller]")]
    public class AdminController : BaseController
    {
        // --- ОРГАНИЗАТОРЫ ---

        [HttpGet("organizers/pending")]
        public async Task<IActionResult> GetPendingOrganizers()
        {
            var result = await Mediator.Send(new GetPendingOrganizersQuery());
            return Ok(result);
        }

        [HttpPost("organizers/{id}/approve")]
        public async Task<IActionResult> ApproveOrganizer(Guid id)
        {
            await Mediator.Send(new ApproveOrganizerCommand(id));
            return Ok(new { Message = AppConstants.Success.ORGANIZER_APPROVED });
        }

        [HttpPost("organizers/{id}/reject")]
        public async Task<IActionResult> RejectOrganizer(Guid id, [FromBody] string reason)
        {
            await Mediator.Send(new RejectOrganizerCommand(id, reason));
            return Ok(new { Message = AppConstants.Success.ORGANIZER_REJECTED });
        }

        // --- ИВЕНТЫ ---

        [HttpGet("events/pending")]
        public async Task<IActionResult> GetPendingEvents()
        {
            var events = await Mediator.Send(new GetPendingEventsQuery());
            return Ok(events);
        }

        [HttpPost("events/{id}/approve")]
        public async Task<IActionResult> ApproveEvent(Guid id)
        {
            await Mediator.Send(new ApproveEventCommand(id));
            return Ok(new { Message = AppConstants.Success.REQUEST_APPROVED });
        }

        [HttpPost("events/{id}/reject")]
        public async Task<IActionResult> RejectEvent(Guid id, [FromBody] string reason)
        {
            await Mediator.Send(new RejectEventCommand(id, reason));
            return Ok(new { Message = AppConstants.Success.ORGANIZER_REJECTED });
        }
    }
}