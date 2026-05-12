using MediatR;

namespace Karakatsiya.Features.Auth.Commands.LoginUser
{
    public record LoginUserCommand(string Email, string Password) : IRequest<(bool Success, string? Token, string MessageKey)>;
}
