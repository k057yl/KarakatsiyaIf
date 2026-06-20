using Karakatsiya.Data;
using Karakatsiya.Data.Entities.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Karakatsiya.Features.Users.Dtos;

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
            return await _db.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new UserProfileDto(
                    u.Id,
                    u.Email,
                    u.Nickname,
                    u.TelegramChatId,
                    new ContactInfo(
                        u.Contacts!.Phone,
                        u.Contacts.Email,
                        u.Contacts.Website,
                        u.Contacts.Telegram,
                        u.Contacts.Instagram
                    )
                ))
                .FirstOrDefaultAsync(ct);
        }
    }
}