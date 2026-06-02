using Karakatsiya.Data;
using Karakatsiya.Data.Entities.Audience;
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
            await _context.Set<TelegramConnectionCode>()
                .Where(c => c.UserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken);

            var code = Random.Shared.Next(100000, 1000000).ToString();

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