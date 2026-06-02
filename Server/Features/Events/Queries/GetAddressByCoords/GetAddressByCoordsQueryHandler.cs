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
            var client = _httpClientFactory.CreateClient();

            var userAgent = _configuration[AppConstants.Config.GEO_USER_AGENT];

            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            }

            var latStr = request.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lonStr = request.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

            var url = $"https://nominatim.openstreetmap.org/reverse?lat={latStr}&lon={lonStr}&format=json&addressdetails=1&accept-language=uk";

            try
            {
                var response = await client.GetAsync(url, ct);
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
