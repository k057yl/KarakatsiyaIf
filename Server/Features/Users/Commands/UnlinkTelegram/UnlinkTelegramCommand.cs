using MediatR;

namespace Karakatsiya.Features.Users.Commands.UnlinkTelegram
{
    public record UnlinkTelegramCommand(Guid UserId) : IRequest<bool>;
}
