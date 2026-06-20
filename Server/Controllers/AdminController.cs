using Karakatsiya.Constants;
using Karakatsiya.Data.Enums;
using Karakatsiya.Features.Admin.Commands.ApproveOrganizer;
using Karakatsiya.Features.Admin.Commands.DeleteCommentByReport;
using Karakatsiya.Features.Admin.Commands.DeleteOrganizer;
using Karakatsiya.Features.Admin.Commands.DeletePerformer;
using Karakatsiya.Features.Admin.Commands.DismissReport;
using Karakatsiya.Features.Admin.Commands.MergePerformer;
using Karakatsiya.Features.Admin.Commands.RejectOrganizer;
using Karakatsiya.Features.Admin.Commands.UpdateOrganizer;
using Karakatsiya.Features.Admin.Commands.VerifyPerformer;
using Karakatsiya.Features.Admin.Queries.GetActiveEvents;
using Karakatsiya.Features.Admin.Queries.GetAllOrganizers;
using Karakatsiya.Features.Admin.Queries.GetAllPerformers;
using Karakatsiya.Features.Admin.Queries.GetPendingOrganizers;
using Karakatsiya.Features.Admin.Queries.GetPendingPerformers;
using Karakatsiya.Features.Admin.Queries.GetReportedComments;
using Karakatsiya.Features.Events.Commands.ApproveEvent;
using Karakatsiya.Features.Events.Commands.DeleteEvent;
using Karakatsiya.Features.Events.Commands.RejectEvent;
using Karakatsiya.Features.Events.Queries.GetPendingEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karakatsiya.Controllers
{
    [Authorize(Roles = nameof(UserRole.SuperAdmin))]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : BaseController
    {
        // --- ОРГАНИЗАТОРЫ ---
        [HttpGet("organizers/pending")]
        public async Task<IActionResult> GetPendingOrganizers() => Ok(await Mediator.Send(new GetPendingOrganizersQuery()));

        [HttpPost("organizers/{id:guid}/approve")]
        public async Task<IActionResult> ApproveOrganizer(Guid id)
        {
            await Mediator.Send(new ApproveOrganizerCommand(id));
            return Ok(new { Message = AppConstants.Success.ORGANIZER_APPROVED });
        }

        [HttpPost("organizers/{id:guid}/reject")]
        public async Task<IActionResult> RejectOrganizer(Guid id, [FromBody] string reason)
        {
            await Mediator.Send(new RejectOrganizerCommand(id, reason));
            return Ok(new { Message = AppConstants.Success.ORGANIZER_REJECTED });
        }

        [HttpGet("organizers")]
        public async Task<IActionResult> GetAllOrganizers([FromQuery] string? search = null) => Ok(await Mediator.Send(new GetAllOrganizersQuery(search)));

        [HttpPut("organizers/{id:guid}")]
        public async Task<IActionResult> UpdateOrganizer(Guid id, [FromBody] UpdateOrganizerCommand command)
        {
            if (id != command.Id) return BadRequest();
            await Mediator.Send(command);
            return Ok(new { Message = AppConstants.Success.CONTACTS_UPDATED });
        }

        [HttpDelete("organizers/{id:guid}")]
        public async Task<IActionResult> DeleteOrganizer(Guid id)
        {
            await Mediator.Send(new DeleteOrganizerCommand(id));
            return Ok(new { Message = AppConstants.Success.ORGANIZER_REJECTED });
        }

        // --- ИВЕНТЫ (МОДЕРАЦИЯ) ---
        [HttpGet("events/pending")]
        public async Task<IActionResult> GetPendingEvents() => Ok(await Mediator.Send(new GetPendingEventsQuery()));

        [HttpPost("events/{id:guid}/approve")]
        public async Task<IActionResult> ApproveEvent(Guid id, [FromQuery] bool isVip = false)
        {
            await Mediator.Send(new ApproveEventCommand(id, isVip));
            return Ok(new { Message = AppConstants.Success.EVENT_APPROVED });
        }

        [HttpPost("events/{id:guid}/reject")]
        public async Task<IActionResult> RejectEvent(Guid id, [FromBody] string reason)
        {
            await Mediator.Send(new RejectEventCommand(id, reason, IsToFix: false));
            return Ok(new { Message = AppConstants.Success.EVENT_REJECTED });
        }

        [HttpPost("events/{id:guid}/fix")]
        public async Task<IActionResult> SendToFix(Guid id, [FromBody] string reason)
        {
            await Mediator.Send(new RejectEventCommand(id, reason, IsToFix: true));
            return Ok(new { Message = AppConstants.Success.EVENT_SENT_TO_FIX });
        }

        [HttpGet("events/active")]
        public async Task<IActionResult> GetActiveEvents() => Ok(await Mediator.Send(new GetActiveEventsQuery()));

        [HttpDelete("events/{id:guid}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            await Mediator.Send(new DeleteEventCommand(id));
            return Ok(new { Message = AppConstants.Success.EVENT_DELETED });
        }

        // --- МОДЕРАЦИЯ КОММЕНТАРИЕВ ---
        [HttpGet("comments/reported")]
        public async Task<IActionResult> GetReportedComments() => Ok(await Mediator.Send(new GetReportedCommentsQuery()));

        [HttpDelete("comments/{id:guid}/confirm-report")]
        public async Task<IActionResult> DeleteCommentByReport(Guid id)
        {
            await Mediator.Send(new DeleteCommentByReportCommand(id));
            return Ok(new { Message = AppConstants.Success.EVENT_DELETED });
        }

        [HttpPost("comments/{id:guid}/dismiss-report")]
        public async Task<IActionResult> DismissReport(Guid id)
        {
            await Mediator.Send(new DismissReportCommand(id));
            return Ok(new { Message = AppConstants.Success.REQUEST_APPROVED });
        }

        // --- УПРАВЛЕНИЕ АРТИСТАМИ ---
        [HttpGet("performers/pending")]
        public async Task<IActionResult> GetPendingPerformers() => Ok(await Mediator.Send(new GetPendingPerformersQuery()));

        [HttpPut("performers/{id:guid}/verify")]
        public async Task<IActionResult> VerifyPerformer(Guid id, [FromBody] VerifyPerformerCommand command)
        {
            await Mediator.Send(command with { Id = id });
            return Ok(new { Message = AppConstants.Success.PERFORMER_VERIFIED });
        }

        [HttpPost("performers/{id:guid}/merge-into/{targetId:guid}")]
        public async Task<IActionResult> MergePerformer(Guid id, Guid targetId)
        {
            await Mediator.Send(new MergePerformerCommand(id, targetId));
            return Ok(new { Message = AppConstants.Success.PERFORMER_MERGED });
        }

        [HttpGet("performers")]
        public async Task<IActionResult> GetAllPerformers([FromQuery] string? search = null) => Ok(await Mediator.Send(new GetAllPerformersQuery(search)));

        [HttpDelete("performers/{id:guid}")]
        public async Task<IActionResult> DeletePerformer(Guid id)
        {
            await Mediator.Send(new DeletePerformerCommand(id));
            return Ok(new { Message = AppConstants.Success.PERFORMER_DELETED });
        }

        // TODO: Метод UploadPerformerAvatar тоже просится на рефакторинг в Command, но пока оставим, чтобы не ломать сборку.
    }
}