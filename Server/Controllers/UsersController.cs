using Karakatsiya.Constants;
using Karakatsiya.Features.Organizers.Commands.ApplyForOrganizer;
using Karakatsiya.Features.Users.Commands.GenerateTelegramOtp;
using Karakatsiya.Features.Users.Commands.UnlinkTelegram;
using Karakatsiya.Features.Users.Commands.UpdateContacts;
using Karakatsiya.Features.Users.Queries.GetMyProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karakatsiya.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : BaseController
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var profile = await Mediator.Send(new GetMyProfileQuery(CurrentUserId));

            if (profile == null)
            {
                return NotFound(new { Message = AppConstants.Errors.USER_NOT_FOUND });
            }

            return Ok(profile);
        }

        [HttpPost("me/apply-organizer")]
        public async Task<IActionResult> ApplyForOrganizer([FromBody] ApplyForOrganizerCommand command)
        {
            if (CurrentUserId == Guid.Empty)
                return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });

            var finalCommand = command with { UserId = CurrentUserId };
            var organizerId = await Mediator.Send(finalCommand);

            return Ok(new { Id = organizerId, Message = AppConstants.Others.APPLICATION_SUCCESS });
        }

        [HttpPost("me/telegram/generate-otp")]
        public async Task<IActionResult> GenerateTelegramOtp()
        {
            var command = new GenerateTelegramOtpCommand(CurrentUserId);
            var code = await Mediator.Send(command);

            return Ok(new { Code = code });
        }

        [HttpPost("me/telegram/unlink")]
        public async Task<IActionResult> UnlinkTelegram()
        {
            var success = await Mediator.Send(new UnlinkTelegramCommand(CurrentUserId));

            if (!success)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("me/contacts")]
        public async Task<IActionResult> UpdateContacts([FromBody] UpdateContactsCommand command)
        {
            if (CurrentUserId == Guid.Empty)
                return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });

            var finalCommand = command with { UserId = CurrentUserId };
            var (success, messageKey) = await Mediator.Send(finalCommand);

            if (!success)
            {
                return BadRequest(new { Message = messageKey });
            }

            return Ok(new { Message = messageKey });
        }
    }
}