using System.Reflection;
using MediatR;
using Karakatsiya.Services.Interfaces;

namespace Karakatsiya.Services.Behaviors
{
    public class SanitizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ISanitizerService _sanitizer;

        public SanitizationBehavior(ISanitizerService sanitizer)
        {
            _sanitizer = sanitizer;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var stringProperties = request.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

            foreach (var prop in stringProperties)
            {
                var originalValue = (string?)prop.GetValue(request);

                if (!string.IsNullOrWhiteSpace(originalValue))
                {
                    var sanitizedValue = _sanitizer.StripAllHtml(originalValue);

                    if (originalValue != sanitizedValue)
                    {
                        prop.SetValue(request, sanitizedValue);
                    }
                }
            }

            return await next();
        }
    }
}