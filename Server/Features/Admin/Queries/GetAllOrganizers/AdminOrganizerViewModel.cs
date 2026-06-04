namespace Karakatsiya.Features.Admin.Queries.GetAllOrganizers
{
    public record AdminOrganizerViewModel(Guid Id, string Name, string? Email, string? Phone, Guid UserId);
}
