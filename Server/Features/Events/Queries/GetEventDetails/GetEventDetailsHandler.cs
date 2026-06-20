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
            int affectedRows = await _context.Events
                .Where(e => e.Id == request.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.ViewsCount, e => e.ViewsCount + 1), cancellationToken);

            if (affectedRows == 0)
            {
                throw new Exception(AppConstants.Errors.EVENT_NOT_FOUND);
            }

            var eventDetails = await _context.Events
                .AsNoTracking()
                .AsSplitQuery()
                .Where(e => e.Id == request.Id)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    e.StartDate,
                    LocationName = e.Location != null ? e.Location.Name : AppConstants.Others.LOCATION_NOT_SPECIFIED,
                    City = e.Location != null ? e.Location.Address.City! : string.Empty,
                    Street = e.Location != null ? e.Location.Address.Street! : string.Empty,
                    HouseNumber = e.Location != null ? e.Location.Address.HouseNumber! : string.Empty,
                    Latitude = e.Location != null ? e.Location.Address.Latitude : null,
                    Longitude = e.Location != null ? e.Location.Address.Longitude : null,
                    OrganizerName = e.Organizer != null ? e.Organizer.Name : AppConstants.Others.ORGANIZER_NOT_SPECIFIED,
                    e.ExternalTicketUrl,
                    e.ContactLinks,
                    e.IsVip,
                    e.ViewsCount,
                    Photos = e.Photos.Select(p => new EventDetailsPhotoDto(p.ImageUrl, p.PublicId, p.IsMain)).ToList(),
                    Comments = e.Comments.Select(c => new EventCommentDto(
                        c.Id,
                        c.User != null ? (c.User.Nickname ?? c.User.Email) : AppConstants.Others.ANONIM,
                        c.Text,
                        c.CreatedAt,
                        c.ShowInstagram && c.User!.Contacts != null ? c.User.Contacts.Instagram : null,
                        c.ShowTelegram && c.User!.Contacts != null ? c.User.Contacts.Telegram : null
                    )).OrderByDescending(c => c.CreatedAt).ToList(),
                    Performers = e.EventPerformers
                        .Where(ep => ep.Performer != null && ep.Performer.IsVerified)
                        .Select(ep => new EventDetailsPerformerDto(
                            ep.Performer!.Id,
                            ep.Performer.Name,
                            ep.Performer.Slug,
                            ep.Performer.AvatarUrl,
                            ep.Performer.Description,
                            ep.Performer.InstagramUrl,
                            ep.Performer.TelegramUrl,
                            ep.Performer.YouTubeUrl
                        )).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (eventDetails == null)
            {
                throw new Exception(AppConstants.Errors.EVENT_NOT_FOUND);
            }

            return new EventDetailsDto(
                eventDetails.Id,
                _sanitizer.StripAllHtml(eventDetails.Title),
                eventDetails.Description,
                eventDetails.StartDate,
                eventDetails.LocationName,
                eventDetails.City,
                eventDetails.Street,
                eventDetails.HouseNumber,
                eventDetails.Latitude,
                eventDetails.Longitude,
                eventDetails.OrganizerName,
                eventDetails.ExternalTicketUrl,
                eventDetails.ContactLinks,
                eventDetails.IsVip,
                eventDetails.Photos,
                eventDetails.Comments,
                eventDetails.ViewsCount,
                eventDetails.Performers
            );
        }
    }
}