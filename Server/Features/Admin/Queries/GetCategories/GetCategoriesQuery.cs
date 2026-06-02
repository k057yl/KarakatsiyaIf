using MediatR;

namespace Karakatsiya.Features.Admin.Queries.GetCategories
{
    public record GetCategoriesQuery : IRequest<List<CategoryViewModel>>;
}
