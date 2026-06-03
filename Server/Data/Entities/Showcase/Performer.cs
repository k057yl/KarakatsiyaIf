using Karakatsiya.Data.Entities.Common;

namespace Karakatsiya.Data.Entities.Showcase
{
    public class Performer : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public bool IsVerified { get; set; } = false;

        public string? Description { get; set; }
        public string? AvatarUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TelegramUrl { get; set; }
        public string? YouTubeUrl { get; set; }

        public List<EventPerformer> EventPerformers { get; set; } = new();
    }
}
