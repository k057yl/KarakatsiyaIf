using System.Collections;
using System.Reflection;
using Karakatsiya.Services.Interfaces;
using MediatR;

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
            SanitizeObject(request);

            return await next();
        }

        private void SanitizeObject(object? obj, int depth = 0)
        {
            if (obj == null || depth > 5) return;

            var type = obj.GetType();

            var stringProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

            foreach (var prop in stringProperties)
            {
                var originalValue = (string?)prop.GetValue(obj);

                if (!string.IsNullOrWhiteSpace(originalValue))
                {
                    var sanitizedValue = _sanitizer.SanitizeHtml(originalValue);

                    if (originalValue != sanitizedValue)
                    {
                        prop.SetValue(obj, sanitizedValue);
                    }
                }
            }

            var complexProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType.IsClass
                         && p.PropertyType != typeof(string)
                         && !typeof(IEnumerable).IsAssignableFrom(p.PropertyType));

            foreach (var prop in complexProperties)
            {
                var nestedObj = prop.GetValue(obj);
                if (nestedObj != null)
                {
                    SanitizeObject(nestedObj, depth + 1);
                }
            }
        }
    }
}