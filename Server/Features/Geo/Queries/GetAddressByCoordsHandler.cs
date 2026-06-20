using Karakatsiya.Constants;
using Karakatsiya.Features.Events.Queries.GetAddressByCoords;
using MediatR;
using System.Globalization;

namespace Karakatsiya.Features.Geo.Queries
{
    public class GetAddressByCoordsHandler : IRequestHandler<GetAddressByCoordsQuery, OsmReverseResponseDto>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public GetAddressByCoordsHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<OsmReverseResponseDto> Handle(GetAddressByCoordsQuery request, CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var latStr = request.Latitude.ToString(CultureInfo.InvariantCulture);
            var lonStr = request.Longitude.ToString(CultureInfo.InvariantCulture);
            var url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={latStr}&lon={lonStr}&addressdetails=1";

            using var message = new HttpRequestMessage(HttpMethod.Get, url);

            var userAgent = _configuration[$"GeoSettings:{AppConstants.Config.GEO_USER_AGENT}"] ?? "KarakatsiyaApp/1.0";
            message.Headers.TryAddWithoutValidation("User-Agent", userAgent);

            try
            {
                using var response = await client.SendAsync(message, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return new OsmReverseResponseDto(string.Empty, new OsmAddressDto(null, null, null, null, null));
                }

                var data = await response.Content.ReadFromJsonAsync<OsmReverseResponseDto>(cancellationToken: cancellationToken);
                return data ?? new OsmReverseResponseDto(string.Empty, new OsmAddressDto(null, null, null, null, null));
            }
            catch (Exception)
            {
                return new OsmReverseResponseDto(string.Empty, new OsmAddressDto(null, null, null, null, null));
            }
        }
    }
}