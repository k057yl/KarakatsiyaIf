namespace Karakatsiya.Features.Events.Commands.UploadOrganizerPhoto
{
    public record UploadPhotoResult(bool Success, string? Url, string? ErrorMessage);
}
