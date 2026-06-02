using Karakatsiya.Data.Entities.Common;
using Karakatsiya.Data.Entities.Showcase;
using Karakatsiya.Data.Entities.ValueObjects;
using Karakatsiya.Data.Enums;

namespace Karakatsiya.Data.Entities.Audience
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; } = string.Empty;
        public string? Nickname { get; set; }

        public AuthProvider Provider { get; set; } = AuthProvider.Local;
        public bool IsEmailVerified { get; set; } = false;

        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiresAt { get; set; }

        public UserRole Role { get; set; } = UserRole.Visitor;

        public ContactInfo? Contacts { get; set; }

        public Organizer? OrganizerProfile { get; set; }
        public List<Ticket> Tickets { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<EventPhoto> Photos { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();

        public long? TelegramChatId { get; set; }
    }
}
