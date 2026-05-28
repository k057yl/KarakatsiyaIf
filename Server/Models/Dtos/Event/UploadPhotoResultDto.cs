namespace Karakatsiya.Models.Dtos.Event
{
    public record UploadPhotoResultDto(bool Success, string? Url, string? ErrorMessage);
}
