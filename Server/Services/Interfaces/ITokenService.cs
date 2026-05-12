using Karakatsiya.Models.Entities.Audience;

namespace Karakatsiya.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
