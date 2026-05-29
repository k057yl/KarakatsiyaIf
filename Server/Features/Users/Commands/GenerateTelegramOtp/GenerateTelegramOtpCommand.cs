using MediatR;

namespace Karakatsiya.Features.Users.Commands.GenerateTelegramOtp
{
    public record GenerateTelegramOtpCommand(Guid UserId) : IRequest<string>;
}
