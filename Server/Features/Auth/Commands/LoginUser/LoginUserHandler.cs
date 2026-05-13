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
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);

            if (user == null || !user.IsEmailVerified)
            {
                return (false, null, null, null, AppConstants.Errors.INVALID_CREDENTIALS);
            }

            if (string.IsNullOrEmpty(user.PasswordHash) || !BC.Verify(request.Password, user.PasswordHash))
            {
                return (false, null, null, null, AppConstants.Errors.INVALID_CREDENTIALS);
            }

            var token = _tokenService.GenerateToken(user);

            return (true, token, user.Email, user.Role.ToString(), AppConstants.Success.REQUEST_APPROVED);
        }
    }
}
