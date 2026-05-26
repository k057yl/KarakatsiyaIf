using Karakatsiya.Features.Users.Commands.UpdateContacts;
using Karakatsiya.Models.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Karakatsiya.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        [HttpPut("me/contacts")]
        public async Task<IActionResult> UpdateContacts([FromBody] UpdateContactsRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var command = new UpdateContactsCommand(
                userId,
                request.Phone,
                request.Website,
                request.Telegram,
                request.Instagram);

            var (success, messageKey) = await Mediator.Send(command);

            if (!success)
            {
                return BadRequest(new { Message = messageKey });
            }

            return Ok(new { Message = messageKey });
        }
    }
}
