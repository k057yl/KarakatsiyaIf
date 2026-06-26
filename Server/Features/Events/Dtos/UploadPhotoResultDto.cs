namespace Karakatsiya.Features.Events.Dtos
{
    public record UploadPhotoResultDto(bool Success, string? Url, string? ErrorMessage);
}
