using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace Karakatsiya.Features.Auth.Commands.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, (bool Success, string? Token, string? Email, string? Role, string MessageKey)>
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;

        public LoginUserHandler(AppDbContext db, ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<(bool Success, string? Token, string? Email, string? Role, string MessageKey)> Handle(LoginUserCommand request, CancellationToken ct)
        {
            const string dummyHash = "$2a$11$KeXvLOmI7mI9.Z4L9A3QSeVf3yDby8C2v.X7r5E1q6h5K1LwZ2XPy";

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);

            string hashToVerify = user != null && !string.IsNullOrEmpty(user.PasswordHash)
                ? user.PasswordHash
                : dummyHash;

            bool isPasswordValid = BC.Verify(request.Password, hashToVerify);

            if (user == null || !user.IsEmailVerified || !isPasswordValid)
            {
                return (false, null, null, null, AppConstants.Errors.INVALID_CREDENTIALS);
            }

            var token = _tokenService.GenerateToken(user);

            return (true, token, user.Email, user.Role.ToString(), AppConstants.Success.REQUEST_APPROVED);
        }
    }
}
