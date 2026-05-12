namespace Karakatsiya.Services.Interfaces
{
    public interface ISanitizerService
    {
        string SanitizeHtml(string input);
        string StripAllHtml(string input);
    }
}
