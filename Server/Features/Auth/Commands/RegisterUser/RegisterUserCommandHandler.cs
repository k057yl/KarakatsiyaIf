using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Models.Enums;
using Karakatsiya.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace Karakatsiya.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, (bool Success, string MessageKey)>
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _emailService;

        public RegisterUserCommandHandler(AppDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<(bool Success, string MessageKey)> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
            if (existingUser != null)
            {
                return (false, AppConstants.Errors.EMAIL_ALREADY_EXISTS);
            }

            var code = new Random().Next(
                AppConstants.Security.OTP_MIN_VALUE,
                AppConstants.Security.OTP_MAX_VALUE).ToString();

            var user = new User
            {
                Email = request.Email,
                PasswordHash = !string.IsNullOrEmpty(request.Password) ? BC.HashPassword(request.Password) : null,
                IsEmailVerified = false,
                VerificationCode = code,
                VerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(AppConstants.Security.OTP_EXPIRY_MINUTES),
                Role = UserRole.Visitor
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            var emailBody = string.Format(
                AppConstants.Email.VERIF_BODY_TEMPLATE,
                code,
                AppConstants.Security.OTP_EXPIRY_MINUTES);

            await _emailService.SendEmailAsync(
                user.Email,
                AppConstants.Email.VERIF_SUBJECT,
                emailBody);

            return (true, AppConstants.Success.VERIFICATION_CODE_SENT);
        }
    }
}
