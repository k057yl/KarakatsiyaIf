namespace Karakatsiya.Models.Dtos.Event
{
    public record EventDetailsPhotoDto(
        string ImageUrl,
        string PublicId,
        bool IsMain
    );
}
