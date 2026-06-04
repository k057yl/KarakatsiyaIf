using Karakatsiya.Constants;
using Karakatsiya.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Admin.Commands.UpdateOrganizer
{
    public class UpdateOrganizerHandler : IRequestHandler<UpdateOrganizerCommand>
    {
        private readonly AppDbContext _context;

        public UpdateOrganizerHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateOrganizerCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _context.Organizers
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (organizer == null)
            {
                throw new InvalidOperationException(AppConstants.Errors.ORGANIZER_NOT_FOUND);
            }

            organizer.Name = request.Name.Trim();
            organizer.Contacts = organizer.Contacts with
            {
                Phone = request.Phone?.Trim(),
                Email = request.Email?.Trim(),
                Website = request.Website?.Trim(),
                Telegram = request.Telegram?.Trim(),
                Instagram = request.Instagram?.Trim()
            };

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
