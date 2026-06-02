using Karakatsiya.Data.Entities.Showcase;
using MediatR;

namespace Karakatsiya.Features.Admin.Commands.CreateCategory
{
    public record CreateCategoryCommand(string Name, string Icon) : IRequest<EventCategory>;
}
