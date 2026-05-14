using MediatR;

namespace Karakatsiya.Features.Admin.Commands.ApproveOrganizer
{
    public record ApproveOrganizerCommand(Guid OrganizerId) : IRequest<Unit>;
}
