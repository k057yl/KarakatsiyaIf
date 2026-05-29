using Karakatsiya.Models.Dtos.User;
using MediatR;

namespace Karakatsiya.Features.Users.Queries.GetMyProfile
{
    public record GetMyProfileQuery(Guid UserId) : IRequest<UserProfileDto?>;
}
