using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Enums;
using Karakatsiya.Data.Entities.Showcase;
using Karakatsiya.Data.Entities.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Organizers.Commands.ApplyForOrganizer
{
    public class ApplyForOrganizerHandler : IRequestHandler<ApplyForOrganizerCommand, Guid>
    {
        private readonly AppDbContext _context;

        public ApplyForOrganizerHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(ApplyForOrganizerCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.OrganizerProfile)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                throw new InvalidOperationException(AppConstants.Errors.USER_NOT_FOUND);

            if (user.Role == UserRole.Organizer || user.Role == UserRole.PendingOrganizer)
                throw new InvalidOperationException(AppConstants.Errors.ALREADY_APPLIED_OR_ADMIN);

            var contactInfo = new ContactInfo(
                Phone: request.Phone,
                Email: request.Email,
                Website: request.Website,
                Telegram: request.Telegram,
                Instagram: request.Instagram
            );

            if (user.OrganizerProfile != null)
            {
                user.OrganizerProfile.Name = request.Name;
                user.OrganizerProfile.Contacts = contactInfo;
                user.OrganizerProfile.IsDeleted = false;
                user.OrganizerProfile.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var newOrganizer = new Organizer
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Name = request.Name,
                    Contacts = contactInfo,
                    IsDeleted = false
                };

                _context.Organizers.Add(newOrganizer);
                user.OrganizerProfile = newOrganizer;
            }

            user.Role = UserRole.PendingOrganizer;

            await _context.SaveChangesAsync(cancellationToken);

            return user.OrganizerProfile.Id;
        }
    }
}