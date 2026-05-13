using Karakatsiya.Features.Auth.Commands.LoginUser;
using Karakatsiya.Features.Auth.Commands.RegisterUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karakatsiya.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var result = await Mediator.Send(command);

            if (!result.Success)
            {
                return Unauthorized(new { Message = result.MessageKey });
            }

            return Ok(new
            {
                Token = result.Token,
                Email = result.Email,
                Role = result.Role
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await Mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(new { Message = result.MessageKey });
            }

            return Ok(new { Message = result.MessageKey });
        }
    }
}
