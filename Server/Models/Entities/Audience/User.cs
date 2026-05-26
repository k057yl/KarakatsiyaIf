using Karakatsiya.Models.Entities.Common;
using Karakatsiya.Models.Entities.Showcase;
using Karakatsiya.Models.Entities.ValueObjects;
using Karakatsiya.Models.Enums;

namespace Karakatsiya.Models.Entities.Audience
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
    }
}
