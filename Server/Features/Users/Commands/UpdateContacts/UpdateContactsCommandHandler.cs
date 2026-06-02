using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Entities.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Users.Commands.UpdateContacts
{
    public class UpdateContactsCommandHandler : IRequestHandler<UpdateContactsCommand, (bool Success, string MessageKey)>
    {
        private readonly AppDbContext _db;

        public UpdateContactsCommandHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(bool Success, string MessageKey)> Handle(UpdateContactsCommand request, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

            if (user == null)
            {
                return (false, AppConstants.Errors.USER_NOT_FOUND);
            }

            var existingEmail = user.Contacts?.Email;

            user.Contacts = new ContactInfo(
                request.Phone,
                existingEmail,
                request.Website,
                request.Telegram,
                request.Instagram
            );

            await _db.SaveChangesAsync(ct);

            return (true, AppConstants.Success.CONTACTS_UPDATED);
        }
    }
}
