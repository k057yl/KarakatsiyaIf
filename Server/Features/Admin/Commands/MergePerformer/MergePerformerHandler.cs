using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Entities.Showcase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.MergePerformer
{
    public class MergePerformerHandler : IRequestHandler<MergePerformerCommand>
    {
        private readonly AppDbContext _context;

        public MergePerformerHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(MergePerformerCommand request, CancellationToken cancellationToken)
        {
            var sourceExists = await _context.Performers.AnyAsync(p => p.Id == request.SourceId, cancellationToken);
            var targetExists = await _context.Performers.AnyAsync(p => p.Id == request.TargetId, cancellationToken);

            if (!sourceExists || !targetExists)
            {
                throw new InvalidOperationException(AppConstants.Errors.PERFORMER_MERGE_FAILED);
            }

            await _context.EventPerformers
                .Where(ep => ep.PerformerId == request.SourceId &&
                             _context.EventPerformers.Any(targetEp => targetEp.EventId == ep.EventId && targetEp.PerformerId == request.TargetId))
                .ExecuteDeleteAsync(cancellationToken);

            await _context.EventPerformers
                .Where(ep => ep.PerformerId == request.SourceId)
                .ExecuteUpdateAsync(s => s.SetProperty(ep => ep.PerformerId, request.TargetId), cancellationToken);

            await _context.Performers
                .Where(p => p.Id == request.SourceId)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}