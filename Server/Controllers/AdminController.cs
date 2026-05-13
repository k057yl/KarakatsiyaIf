using Karakatsiya.Constants;
using Karakatsiya.Features.Admin.Commands;
using Karakatsiya.Features.Admin.Queries.GetPendingOrganizers;
using Karakatsiya.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karakatsiya.Controllers
{
    [Authorize(Roles = nameof(UserRole.SuperAdmin))]
    [Route("api/[controller]")]
    public class AdminController : BaseController
    {
        [HttpGet("organizers/pending")]
        public async Task<IActionResult> GetPendingOrganizers()
        {
            var query = new GetPendingOrganizersQuery();
            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("organizers/{id}/approve")]
        public async Task<IActionResult> ApproveOrganizer(Guid id)
        {
            var command = new ApproveOrganizerCommand(id);

            await Mediator.Send(command);

            return Ok(new { Message = AppConstants.Success.ORGANIZER_APPROVED });
        }
    }
}