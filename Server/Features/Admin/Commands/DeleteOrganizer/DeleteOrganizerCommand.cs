using MediatR;

namespace Karakatsiya.Features.Admin.Commands.DeleteOrganizer
{
    public record DeleteOrganizerCommand(Guid Id) : IRequest;
}
