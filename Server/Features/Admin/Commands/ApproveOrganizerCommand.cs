using MediatR;

namespace Karakatsiya.Features.Admin.Commands
{
    public record ApproveOrganizerCommand(Guid OrganizerId) : IRequest<Unit>;
}
