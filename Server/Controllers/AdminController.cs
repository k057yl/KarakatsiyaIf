using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Features.Admin.Commands.ApproveOrganizer;
using Karakatsiya.Features.Admin.Commands.RejectOrganizer;
using Karakatsiya.Features.Admin.Queries.GetActiveEvents;
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
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

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

        // --- ИВЕНТЫ (МОДЕРАЦИЯ) ---

        [HttpGet("events/pending")]
        public async Task<IActionResult> GetPendingEvents()
        {
            var events = await Mediator.Send(new GetPendingEventsQuery());
            return Ok(events);
        }

        [HttpPost("events/{eventId:guid}/approve")]
        public async Task<IActionResult> ApproveEvent(Guid eventId, [FromQuery] bool isVip = false)
        {
            var command = new ApproveEventCommand(eventId, isVip);
            await Mediator.Send(command);
            return Ok(new { Message = AppConstants.Success.EVENT_APPROVED });
        }

        [HttpPost("events/{id}/reject")]
        public async Task<IActionResult> RejectEvent(Guid id, [FromBody] string reason)
        {
            await Mediator.Send(new RejectEventCommand(id, reason));
            return Ok(new { Message = AppConstants.Success.EVENT_REJECTED });
        }

        [HttpGet("events/active")]
        public async Task<IActionResult> GetActiveEvents()
        {
            var events = await Mediator.Send(new GetActiveEventsQuery());
            return Ok(events);
        }

        // --- УПРАВЛЕНИЕ СУДЬБАМИ ИВЕНТОВ (ВОЗВРАЩАЕМ НА МЕСТО) ---

        [HttpDelete("events/{id:guid}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound(new { Message = AppConstants.Errors.EVENT_NOT_FOUND });

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return Ok(new { Message = AppConstants.Success.EVENT_DELETED });
        }

        [HttpPost("events/{id:guid}/send-to-fix")]
        public async Task<IActionResult> SendToFix(Guid id, [FromBody] string reason)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound(new { Message = AppConstants.Errors.EVENT_NOT_FOUND });

            ev.Status = EventStatus.Draft;
            await _context.SaveChangesAsync();

            return Ok(new { Message = AppConstants.Success.EVENT_SENT_TO_FIX });
        }

        [HttpPost("events/{id:guid}/toggle-vip")]
        public async Task<IActionResult> ToggleVipStatus(Guid id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound(new { Message = AppConstants.Errors.EVENT_NOT_FOUND });

            ev.IsVip = !ev.IsVip;
            await _context.SaveChangesAsync();

            return Ok(new { IsVip = ev.IsVip, Message = AppConstants.Success.EVENT_VIP_TOGGLED });
        }
    }
}