using MediatR;

namespace Karakatsiya.Features.Admin.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : IRequest;
}
