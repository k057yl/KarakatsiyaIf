using Karakatsiya.Features.Organizers.Commands.ApplyForOrganizer;
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

            return Ok(new { Id = organizerId, Message = "Заявка подана успешно" });
        }
    }
}
