using Karakatsiya.Data;
using Karakatsiya.Models.Entities.Audience;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Users.Commands.GenerateTelegramOtp
{
    public class GenerateTelegramOtpHandler : IRequestHandler<GenerateTelegramOtpCommand, string>
    {
        private readonly AppDbContext _context;

        public GenerateTelegramOtpHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(GenerateTelegramOtpCommand request, CancellationToken cancellationToken)
        {
            var oldCodes = await _context.Set<TelegramConnectionCode>()
                .Where(c => c.UserId == request.UserId)
                .ToListAsync(cancellationToken);

            _context.Set<TelegramConnectionCode>().RemoveRange(oldCodes);

            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            var connectionCode = new TelegramConnectionCode
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Code = code,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };

            await _context.Set<TelegramConnectionCode>().AddAsync(connectionCode, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return code;
        }
    }
}
