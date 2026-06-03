using MediatR;

namespace Karakatsiya.Features.Admin.Commands.DeletePerformer
{
    public record DeletePerformerCommand(Guid Id) : IRequest;
}
