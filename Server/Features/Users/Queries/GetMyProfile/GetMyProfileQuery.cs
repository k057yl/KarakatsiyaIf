using Karakatsiya.Features.Users.Dtos;
using MediatR;

namespace Karakatsiya.Features.Users.Queries.GetMyProfile
{
    public record GetMyProfileQuery(Guid UserId) : IRequest<UserProfileDto?>;
}
