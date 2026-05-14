using MediatR;

namespace Karakatsiya.Features.Admin.Commands.RejectOrganizer
{
    public record RejectOrganizerCommand(Guid OrganizerId, string Reason) : IRequest<Unit>;
}
