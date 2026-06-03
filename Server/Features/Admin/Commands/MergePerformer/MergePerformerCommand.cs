using MediatR;

namespace Karakatsiya.Features.Admin.Commands.MergePerformer
{
    public record MergePerformerCommand(Guid SourceId, Guid TargetId) : IRequest;
}
