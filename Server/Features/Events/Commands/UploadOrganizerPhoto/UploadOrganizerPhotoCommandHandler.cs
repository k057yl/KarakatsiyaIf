using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Events.Commands.UploadOrganizerPhoto
{
    public class UploadOrganizerPhotoCommandHandler : IRequestHandler<UploadOrganizerPhotoCommand, UploadPhotoResult>
    {
        private readonly AppDbContext _context;
        private readonly IPhotoService _photoService;

        public UploadOrganizerPhotoCommandHandler(AppDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        public async Task<UploadPhotoResult> Handle(UploadOrganizerPhotoCommand request, CancellationToken cancellationToken)
        {
            var realOrganizerId = await _context.Organizers
                .Where(o => o.UserId == request.UserId)
                .Select(o => o.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var ev = await _context.Events
                .Include(e => e.Photos)
                .FirstOrDefaultAsync(e => e.Id == request.EventId && e.OrganizerId == realOrganizerId, cancellationToken);

            if (ev == null)
            {
                return new UploadPhotoResult(false, null, AppConstants.Errors.VALIDATION_FAILED);
            }

            int currentPhotosCount = ev.Photos.Count(p => p.IsMain == request.IsMain);
            int maxAllowed = request.IsMain ? 1 : 5;

            if (currentPhotosCount >= maxAllowed)
            {
                return new UploadPhotoResult(false, null, AppConstants.Errors.VALIDATION_FAILED);
            }

            var uploadResult = await _photoService.AddPhotoAsync(request.File, request.IsMain);

            if (uploadResult.Error != null)
            {
                return new UploadPhotoResult(false, null, uploadResult.Error.Message);
            }

            var eventPhoto = new EventPhoto
            {
                Id = Guid.NewGuid(),
                EventId = ev.Id,
                UserId = request.UserId,
                ImageUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId,
                IsMain = request.IsMain,
                IsApproved = true
            };

            _context.EventPhotos.Add(eventPhoto);
            await _context.SaveChangesAsync(cancellationToken);

            return new UploadPhotoResult(true, eventPhoto.ImageUrl, null);
        }
    }
}