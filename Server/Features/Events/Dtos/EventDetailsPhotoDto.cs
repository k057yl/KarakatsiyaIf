namespace Karakatsiya.Features.Events.Dtos
{
    public record EventDetailsPhotoDto(
        string ImageUrl,
        string PublicId,
        bool IsMain
    );
}
