namespace Karakatsiya.Features.Admin.Dtos
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
