using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Features.Events.Dtos;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Queries.GetEventDetails
{
    public class GetEventDetailsHandler : IRequestHandler<GetEventDetailsQuery, EventDetailsDto>
    {
        private readonly AppDbContext _context;
        private readonly ISanitizerService _sanitizer;

        public GetEventDetailsHandler(AppDbContext context, ISanitizerService sanitizer)
        {
            _context = context;
            _sanitizer = sanitizer;
        }

        public async Task<EventDetailsDto> Handle(GetEventDetailsQuery request, CancellationToken cancellationToken)
        {
            var ev = await _context.Events
                .Include(e => e.Location)
                .Include(e => e.Organizer)
                .Include(e => e.Photos)
                .Include(e => e.EventPerformers).ThenInclude(ep => ep.Performer)
                .Include(e => e.Comments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (ev == null)
            {
                throw new Exception(AppConstants.Errors.EVENT_NOT_FOUND);
            }

            ev.ViewsCount++;
            await _context.SaveChangesAsync(cancellationToken);

            var photosDto = ev.Photos != null
                ? ev.Photos.Select(p => new EventDetailsPhotoDto(p.ImageUrl, p.PublicId, p.IsMain)).ToList()
                : new List<EventDetailsPhotoDto>();

            var commentsDto = ev.Comments != null
                ? ev.Comments.Select(c => new EventCommentDto(
                    c.Id,
                    c.User != null ? (c.User.Nickname ?? c.User.Email) : AppConstants.Others.ANONIM,
                    c.Text,
                    c.CreatedAt,
                    c.ShowInstagram && c.User?.Contacts != null ? c.User.Contacts.Instagram : null,
                    c.ShowTelegram && c.User?.Contacts != null ? c.User.Contacts.Telegram : null
                  )).OrderByDescending(c => c.CreatedAt).ToList()
                : new List<EventCommentDto>();

            var performersDto = ev.EventPerformers != null
                ? ev.EventPerformers
                    .Where(ep => ep.Performer != null && ep.Performer.IsVerified)
                    .Select(ep => new EventDetailsPerformerDto(
                        ep.Performer.Id,
                        ep.Performer.Name,
                        ep.Performer.Slug,
                        ep.Performer.AvatarUrl,
                        ep.Performer.Description,
                        ep.Performer.InstagramUrl,
                        ep.Performer.TelegramUrl,
                        ep.Performer.YouTubeUrl
                    )).ToList()
                : new List<EventDetailsPerformerDto>();

            return new EventDetailsDto(
                ev.Id,
                _sanitizer.StripAllHtml(ev.Title),
                ev.Description,
                ev.StartDate,
                ev.Location != null ? ev.Location.Name : AppConstants.Others.LOCATION_NOT_SPECIFIED,
                ev.Location != null ? ev.Location.Address.City : string.Empty,
                ev.Location != null ? ev.Location.Address.Street : string.Empty,
                ev.Location != null ? ev.Location.Address.HouseNumber : string.Empty,
                ev.Location?.Address.Latitude,
                ev.Location?.Address.Longitude,
                ev.Organizer != null ? ev.Organizer.Name : AppConstants.Others.ORGANIZER_NOT_SPECIFIED,
                ev.ExternalTicketUrl,
                ev.ContactLinks,
                ev.IsVip,
                photosDto,
                commentsDto,
                ev.ViewsCount,
                performersDto
            );
        }
    }
}