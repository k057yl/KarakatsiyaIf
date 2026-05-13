using Karakatsiya.Constants;
using Karakatsiya.Features.Organizers.Commands.ApplyForOrganizer;
using Karakatsiya.Models.Dtos.Organizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karakatsiya.Controllers
{
    [Authorize]
    public class OrganizersController : BaseController
    {
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForOrganizer([FromBody] ApplyForOrganizerDto dto)
        {
            if (CurrentUserId == Guid.Empty)
                return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });

            var command = new ApplyForOrganizerCommand(
                UserId: CurrentUserId,
                Name: dto.Name,
                Phone: dto.Phone,
                Email: dto.Email,
                Website: dto.Website,
                Telegram: dto.Telegram,
                Instagram: dto.Instagram
            );

            var organizerId = await Mediator.Send(command);

            return Ok(new
            {
                Message = AppConstants.Success.APPLICATION_SUBMITTED,
                OrganizerId = organizerId
            });
        }
    }
}
