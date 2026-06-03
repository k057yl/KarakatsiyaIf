using MediatR;

namespace Karakatsiya.Features.Performers.Commands.CreatePerformer
{
    public record CreatePerformerCommand(string Name) : IRequest<Guid>;
}
