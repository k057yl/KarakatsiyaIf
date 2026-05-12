using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Auth.Commands.VerifyCode
{
    public class VerifyCodeHandler : IRequestHandler<VerifyCodeCommand, (bool Success, string? Token, string MessageKey)>
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;

        public VerifyCodeHandler(AppDbContext db, ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<(bool Success, string? Token, string MessageKey)> Handle(VerifyCodeCommand request, CancellationToken ct)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsEmailVerified, ct);

            if (user == null)
            {
                return (false, null, AppConstants.Errors.USER_NOT_FOUND_OR_ALREADY_VERIFIED);
            }

            if (user.VerificationCodeExpiresAt < DateTime.UtcNow)
            {
                return (false, null, AppConstants.Errors.VERIFICATION_CODE_EXPIRED);
            }

            if (user.VerificationCode != request.Code)
            {
                return (false, null, AppConstants.Errors.INVALID_VERIFICATION_CODE);
            }

            user.IsEmailVerified = true;
            user.VerificationCode = null;
            user.VerificationCodeExpiresAt = null;

            await _db.SaveChangesAsync(ct);

            var token = _tokenService.GenerateToken(user);

            return (true, token, AppConstants.Success.REQUEST_APPROVED);
        }
    }
}
