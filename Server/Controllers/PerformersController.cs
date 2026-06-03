using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Features.Performers.Commands.CreatePerformer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/performers")]
    public class PerformersController : BaseController
    {
        private readonly AppDbContext _context;

        public PerformersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetLookupList()
        {
            var list = await _context.Performers
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePerformerCommand command)
        {
            var performerId = await Mediator.Send(command);
            return Ok(new { Id = performerId, Message = AppConstants.Success.PERFORMER_PENDING_MODERATION });
        }
    }
}
