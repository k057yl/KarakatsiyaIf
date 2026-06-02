using MediatR;

namespace Karakatsiya.Features.Events.Commands.UploadOrganizerPhoto
{
    public record UploadOrganizerPhotoCommand(Guid EventId, Guid UserId, IFormFile File, bool IsMain) : IRequest<UploadPhotoResultDto>;
}
