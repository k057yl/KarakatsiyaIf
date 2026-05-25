namespace Karakatsiya.Models.Dtos.Event
{
    public record CreateEventPhotoDto(
        string ImageUrl,
        string PublicId,
        bool IsMain
    );
}
