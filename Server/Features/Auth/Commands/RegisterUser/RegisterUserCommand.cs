using MediatR;

namespace Karakatsiya.Features.Auth.Commands.RegisterUser
{
    public record RegisterUserCommand(string Email, string? Password) : IRequest<(bool Success, string MessageKey)>;
}
