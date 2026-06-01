using Karakatsiya.Services.Interfaces;
using MediatR;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace Karakatsiya.Services.Behaviors
{
    public class SanitizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ISanitizerService _sanitizer;
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _stringPropsCache = new();
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _complexPropsCache = new();

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

            var stringProperties = _stringPropsCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(string) && p.CanWrite)
                .ToArray());

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

            var complexProperties = _complexPropsCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType.IsClass
                         && p.PropertyType != typeof(string)
                         && !typeof(IEnumerable).IsAssignableFrom(p.PropertyType))
                .ToArray());

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