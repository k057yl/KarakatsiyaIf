namespace Karakatsiya.Features.Events.Commands.UploadOrganizerPhoto
{
    public record UploadPhotoResultDto(bool Success, string? Url, string? ErrorMessage);
}
