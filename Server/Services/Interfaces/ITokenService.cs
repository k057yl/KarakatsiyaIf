using Karakatsiya.Data.Entities.Audience;
using System.Security.Claims;

namespace Karakatsiya.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
