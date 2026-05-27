using Karakatsiya.Features.Comments.Commands.CreateComment;
using Karakatsiya.Features.Comments.Commands.ReportComment;
using Karakatsiya.Models.Dtos.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Karakatsiya.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CommentsController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var command = new CreateCommentCommand(
                userId,
                request.EventId,
                request.Text,
                request.ShowInstagram,
                request.ShowTelegram
            );

            var (success, commentId, messageKey) = await Mediator.Send(command);

            if (!success)
            {
                return BadRequest(new { Message = messageKey });
            }

            return Ok(new { CommentId = commentId, Message = messageKey });
        }

        [HttpPost("{id}/report")]
        public async Task<IActionResult> ReportComment(Guid id, [FromBody] ReportCommentRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var command = new ReportCommentCommand(id, userId, request.Reason);
            var (success, messageKey) = await Mediator.Send(command);

            if (!success) return BadRequest(new { Message = messageKey });
            return Ok(new { Message = messageKey });
        }
    }
}
