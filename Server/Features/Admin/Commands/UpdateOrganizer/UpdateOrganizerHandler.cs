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

            organizer.Name = request.Name;
            organizer.Contacts = organizer.Contacts with
            {
                Phone = request.Phone,
                Email = request.Email,
                Website = request.Website,
                Telegram = request.Telegram,
                Instagram = request.Instagram
            };

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
