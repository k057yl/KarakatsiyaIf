using Karakatsiya.Models.Entities.Showcase;
using Karakatsiya.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karakatsiya.Data.Configurations
{
    public class OrganizerConfiguration : IEntityTypeConfiguration<Organizer>
    {
        public void Configure(EntityTypeBuilder<Organizer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(AppConstants.Validation.MAX_NAME_LENGTH);

            builder.OwnsOne(x => x.Contacts, c =>
            {
                c.Property(p => p.Phone)
                    .HasMaxLength(AppConstants.Validation.MAX_PHONE_LENGTH)
                    .HasColumnName(AppConstants.Columns.CONTACT_PHONE);

                c.Property(p => p.Email)
                    .HasMaxLength(AppConstants.Validation.MAX_EMAIL_LENGTH)
                    .HasColumnName(AppConstants.Columns.CONTACT_EMAIL);
            });
        }
    }
}