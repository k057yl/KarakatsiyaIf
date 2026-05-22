namespace Karakatsiya.Models.Dtos.Event
{
    public record AdminActiveEventDto(
        Guid Id,
        string Title,
        DateTime StartDate,
        string LocationName,
        string City,
        bool IsVip
    );
}
