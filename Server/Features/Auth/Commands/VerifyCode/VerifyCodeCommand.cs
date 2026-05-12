using MediatR;

namespace Karakatsiya.Features.Auth.Commands.VerifyCode
{
    public record VerifyCodeCommand(string Email, string Code) : IRequest<(bool Success, string? Token, string MessageKey)>;
}
