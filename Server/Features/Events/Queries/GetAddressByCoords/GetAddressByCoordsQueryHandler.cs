using Karakatsiya.Constants;
using MediatR;

namespace Karakatsiya.Features.Events.Queries.GetAddressByCoords
{
    public class GetAddressByCoordsQueryHandler : IRequestHandler<GetAddressByCoordsQuery, OsmReverseResponseDto?>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public GetAddressByCoordsQueryHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<OsmReverseResponseDto?> Handle(GetAddressByCoordsQuery request, CancellationToken ct)
        {
            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var latStr = request.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lonStr = request.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var url = $"https://nominatim.openstreetmap.org/reverse?lat={latStr}&lon={lonStr}&format=json&addressdetails=1&accept-language=uk";

            using var message = new HttpRequestMessage(HttpMethod.Get, url);

            var userAgent = _configuration[AppConstants.Config.GEO_USER_AGENT];

            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                message.Headers.TryAddWithoutValidation("User-Agent", userAgent);
            }

            try
            {
                using var response = await client.SendAsync(message, ct);
                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<OsmReverseResponseDto>(cancellationToken: ct);
            }
            catch
            {
                return null;
            }
        }
    }
}
