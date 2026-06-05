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
        public async Task<IActionResult> GetPendingOrganizers()
        {
            var result = await Mediator.Send(new GetPendingOrganizersQuery());
            return Ok(result);
        }

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
        public async Task<IActionResult> GetAllOrganizers([FromQuery] string? search = null)
        {
            var result = await Mediator.Send(new GetAllOrganizersQuery(search));
            return Ok(result);
        }

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
        public async Task<IActionResult> GetPendingEvents()
        {
            var events = await Mediator.Send(new GetPendingEventsQuery());
            return Ok(events);
        }

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
        public async Task<IActionResult> GetActiveEvents()
        {
            var events = await Mediator.Send(new GetActiveEventsQuery());
            return Ok(events);
        }

        [HttpDelete("events/{id:guid}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            return Ok(new { Message = AppConstants.Success.EVENT_DELETED });
        }

        [HttpPost("events/{id:guid}/toggle-vip")]
        public async Task<IActionResult> ToggleVipStatus(Guid id)
        {
            return Ok(new { Message = AppConstants.Success.EVENT_VIP_TOGGLED });
        }

        // --- МОДЕРАЦИЯ КОММЕНТАРИЕВ (ЖАЛОБЫ) ---

        [HttpGet("comments/reported")]
        public async Task<IActionResult> GetReportedComments()
        {
            var result = await Mediator.Send(new GetReportedCommentsQuery());
            return Ok(result);
        }

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
        public async Task<IActionResult> GetPendingPerformers()
        {
            var result = await Mediator.Send(new GetPendingPerformersQuery());
            return Ok(result);
        }

        [HttpPut("performers/{id:guid}/verify")]
        public async Task<IActionResult> VerifyPerformer(Guid id, [FromBody] VerifyPerformerCommand command)
        {
            var finalCommand = command with { Id = id };
            await Mediator.Send(finalCommand);
            return Ok(new { Message = AppConstants.Success.PERFORMER_VERIFIED });
        }

        [HttpPost("performers/{id:guid}/merge-into/{targetId:guid}")]
        public async Task<IActionResult> MergePerformer(Guid id, Guid targetId)
        {
            await Mediator.Send(new MergePerformerCommand(id, targetId));
            return Ok(new { Message = AppConstants.Success.PERFORMER_MERGED });
        }

        [HttpGet("performers")]
        public async Task<IActionResult> GetAllPerformers([FromQuery] string? search = null)
        {
            var result = await Mediator.Send(new GetAllPerformersQuery(search));
            return Ok(result);
        }

        [HttpDelete("performers/{id:guid}")]
        public async Task<IActionResult> DeletePerformer(Guid id)
        {
            await Mediator.Send(new DeletePerformerCommand(id));
            return Ok(new { Message = AppConstants.Success.PERFORMER_DELETED });
        }

        [HttpPost("performers/upload-avatar")]
        public async Task<IActionResult> UploadPerformerAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = AppConstants.Errors.FILE_MISSING });

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!AppConstants.Storage.ALLOWED_EXTENSIONS.Contains(extension))
                return BadRequest(new { Message = AppConstants.Errors.INVALID_IMAGE_FORMAT });

            var fileName = $"{Guid.NewGuid()}{extension}";

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                AppConstants.Storage.WWWROOT_FOLDER,
                AppConstants.Storage.UPLOADS_FOLDER,
                AppConstants.Storage.USER_PHOTOS_SUBFOLDER
            );

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativeUrl = $"/{AppConstants.Storage.UPLOADS_FOLDER}/{AppConstants.Storage.USER_PHOTOS_SUBFOLDER}/{fileName}";
            return Ok(new { Url = relativeUrl });
        }
    }
}