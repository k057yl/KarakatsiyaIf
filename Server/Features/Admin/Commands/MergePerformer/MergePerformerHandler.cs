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
            var source = await _context.Performers
                .FirstOrDefaultAsync(p => p.Id == request.SourceId, cancellationToken);

            var target = await _context.Performers
                .FirstOrDefaultAsync(p => p.Id == request.TargetId, cancellationToken);

            if (source == null || target == null)
            {
                throw new InvalidOperationException(AppConstants.Errors.PERFORMER_MERGE_FAILED);
            }

            var oldLinks = await _context.EventPerformers
                .Where(ep => ep.PerformerId == request.SourceId)
                .ToListAsync(cancellationToken);

            foreach (var link in oldLinks)
            {
                var alreadyExists = await _context.EventPerformers
                    .AnyAsync(ep => ep.EventId == link.EventId && ep.PerformerId == request.TargetId, cancellationToken);

                if (!alreadyExists)
                {
                    _context.EventPerformers.Add(new EventPerformer
                    {
                        EventId = link.EventId,
                        PerformerId = request.TargetId
                    });
                }

                _context.EventPerformers.Remove(link);
            }

            _context.Performers.Remove(source);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}