using Karakatsiya.Data;
using Karakatsiya.Models.Dtos.User;
using Karakatsiya.Models.Entities.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Users.Queries.GetMyProfile
{
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, UserProfileDto?>
    {
        private readonly AppDbContext _db;

        public GetMyProfileQueryHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserProfileDto?> Handle(GetMyProfileQuery request, CancellationToken ct)
        {
            var rawData = await _db.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Nickname,
                    u.TelegramChatId,
                    Phone = u.Contacts != null ? u.Contacts.Phone : null,
                    EmailContact = u.Contacts != null ? u.Contacts.Email : null,
                    Website = u.Contacts != null ? u.Contacts.Website : null,
                    Telegram = u.Contacts != null ? u.Contacts.Telegram : null,
                    Instagram = u.Contacts != null ? u.Contacts.Instagram : null
                })
                .FirstOrDefaultAsync(ct);

            if (rawData == null) return null;

            return new UserProfileDto(
                rawData.Id,
                rawData.Email,
                rawData.Nickname,
                rawData.TelegramChatId,
                new ContactInfo(
                    rawData.Phone,
                    rawData.EmailContact,
                    rawData.Website,
                    rawData.Telegram,
                    rawData.Instagram
                )
            );
        }
    }
}
