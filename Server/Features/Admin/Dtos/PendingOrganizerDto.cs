namespace Karakatsiya.Features.Admin.Dtos
{
    public record PendingOrganizerDto(
        Guid UserId,
        Guid OrganizerId,
        string Name,
        string? Phone,
        string? Email,
        string? Telegram,
        DateTime AppliedAt
    );
}
