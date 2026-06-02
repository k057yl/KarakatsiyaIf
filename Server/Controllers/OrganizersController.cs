using Karakatsiya.Constants;
using Karakatsiya.Features.Organizers.Commands.ApplyForOrganizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karakatsiya.Controllers
{
    [Authorize]
    public class OrganizersController : BaseController
    {
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForOrganizer([FromBody] ApplyForOrganizerCommand command)
        {
            if (CurrentUserId == Guid.Empty)
                return Unauthorized(new { Message = AppConstants.Errors.INVALID_TOKEN });

            var finalCommand = command with { UserId = CurrentUserId };
            var organizerId = await Mediator.Send(finalCommand);

            return Ok(new
            {
                Message = AppConstants.Success.APPLICATION_SUBMITTED,
                OrganizerId = organizerId
            });
        }
    }
}