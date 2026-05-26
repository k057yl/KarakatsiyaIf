using Karakatsiya.Constants;
using Karakatsiya.Models.Entities.Audience;
using Karakatsiya.Models.Entities.Showcase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karakatsiya.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(AppConstants.Validation.MAX_EMAIL_LENGTH);

            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Nickname)
                .HasMaxLength(AppConstants.Validation.MAX_NAME_LENGTH);

            builder.HasOne(x => x.OrganizerProfile)
                .WithOne(x => x.User)
                .HasForeignKey<Organizer>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(x => x.Contacts, contacts =>
            {
                contacts.ToJson();

                contacts.Property(c => c.Phone)
                    .HasMaxLength(AppConstants.Validation.MAX_PHONE_LENGTH);

                contacts.Property(c => c.Email)
                    .HasMaxLength(AppConstants.Validation.MAX_EMAIL_LENGTH);

                contacts.Property(c => c.Website)
                    .HasMaxLength(AppConstants.Validation.MAX_URL_LENGTH);

                contacts.Property(c => c.Telegram)
                    .HasMaxLength(AppConstants.Validation.MAX_SOCIAL_LENGTH);

                contacts.Property(c => c.Instagram)
                    .HasMaxLength(AppConstants.Validation.MAX_SOCIAL_LENGTH);
            });
        }
    }
}