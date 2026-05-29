using Karakatsiya.Constants;
using Karakatsiya.Features.Organizers.Commands.ApplyForOrganizer;
using Karakatsiya.Features.Users.Commands.GenerateTelegramOtp;
using Karakatsiya.Models.Dtos.Organizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karakatsiya.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users/me")]
    public class UsersController : BaseController
    {
        [HttpPost("apply-organizer")]
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

        [HttpPost("telegram/generate-otp")]
        public async Task<IActionResult> GenerateTelegramOtp()
        {
            var command = new GenerateTelegramOtpCommand(CurrentUserId);
            var code = await Mediator.Send(command);

            return Ok(new { Code = code });
        }
    }
}