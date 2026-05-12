using Ganss.Xss;
using Karakatsiya.Services.Interfaces;
using System.Text.RegularExpressions;

namespace Karakatsiya.Services
{
    public class SanitizerService : ISanitizerService
    {
        private readonly HtmlSanitizer _sanitizer;

        public SanitizerService()
        {
            _sanitizer = new HtmlSanitizer();
        }

        public string SanitizeHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return _sanitizer.Sanitize(input);
        }

        public string StripAllHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return Regex.Replace(input, "<.*?>", string.Empty).Trim();
        }
    }
}
