using Karakatsiya.Constants;
using Karakatsiya.Features.Organizers.Commands.ApplyForOrganizer;
using Karakatsiya.Features.Users.Commands.GenerateTelegramOtp;
using Karakatsiya.Features.Users.Commands.UnlinkTelegram;
using Karakatsiya.Features.Users.Commands.UpdateContacts;
using Karakatsiya.Features.Users.Queries.GetMyProfile;
using Karakatsiya.Models.Dtos.Organizer;
using Karakatsiya.Models.Dtos.User;
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
        public async Task<IActionResult> ApplyForOrganizer([FromBody] ApplyOrganizerRequestDto request)
        {
            var command = new ApplyForOrganizerCommand(
                UserId: CurrentUserId,
                Name: request.Name,
                Phone: request.Phone,
                Email: request.Email,
                Website: request.Website,
                Telegram: request.Telegram,
                Instagram: request.Instagram
            );

            var organizerId = await Mediator.Send(command);
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
        public async Task<IActionResult> UpdateContacts([FromBody] UpdateContactsRequest request)
        {
            var command = new UpdateContactsCommand(
                CurrentUserId,
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