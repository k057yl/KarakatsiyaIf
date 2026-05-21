using Karakatsiya.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Karakatsiya.Controllers
{
    [Route("api/[controller]")]
    public class PaymentsController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public PaymentsController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("checkout/{eventId:guid}")]
        public IActionResult GetCheckoutData(Guid eventId)
        {
            var useMock = _configuration.GetValue<bool>("WayForPay:UseMockPayment");

            if (useMock)
            {
                return Ok(new { useMock = true, eventId = eventId });
            }

            var merchantAccount = _configuration["WayForPay:MerchantAccount"];
            var secretKey = _configuration["WayForPay:SecretKey"];
            var merchantDomain = _configuration["WayForPay:MerchantDomain"];

            var orderReference = $"{eventId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var orderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var amount = "500.00";
            var currency = "UAH";

            var productName = "VIP";
            var productCount = "1";
            var productPrice = "500.00";

            var stringToSign = $"{merchantAccount};{merchantDomain};{orderReference};{orderDate};{amount};{currency};{productName};{productCount};{productPrice}";
            var signature = GenerateSignature(stringToSign, secretKey!);

            return Ok(new
            {
                useMock = false,
                merchantAccount,
                merchantDomainName = merchantDomain,
                orderReference,
                orderDate,
                amount,
                currency,
                productName = new[] { productName },
                productCount = new[] { productCount },
                productPrice = new[] { productPrice },
                merchantSignature = signature,
                returnUrl = $"{merchantDomain}/organizer/dashboard",
                serviceUrl = $"{merchantDomain}/api/payments/callback"
            });
        }

        [HttpPost("fake-success/{eventId:guid}")]
        public async Task<IActionResult> FakeSuccessPayment(Guid eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound();

            ev.IsVipRequested = true;
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpPost("callback")]
        public async Task<IActionResult> WayForPayCallback([FromForm] IFormCollection form)
        {
            var secretKey = _configuration["WayForPay:SecretKey"];
            var transactionStatus = form["transactionStatus"].ToString();
            var orderReference = form["orderReference"].ToString();

            if (transactionStatus != "Approved") return Ok();

            var eventIdString = orderReference.Split('_')[0];
            if (Guid.TryParse(eventIdString, out Guid eventId))
            {
                var ev = await _context.Events.FindAsync(eventId);
                if (ev != null)
                {
                    ev.IsVipRequested = true;
                    await _context.SaveChangesAsync();
                }
            }

            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var responseSignature = GenerateSignature($"{orderReference};accept;{time}", secretKey!);

            return Ok(new { orderReference, status = "accept", time, signature = responseSignature });
        }

        private string GenerateSignature(string data, string secret)
        {
            using var hmac = new HMACMD5(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return string.Join("", hash.Select(b => b.ToString("x2")));
        }
    }
}