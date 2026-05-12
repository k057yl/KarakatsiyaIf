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
            builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Email).IsUnique();

            builder.HasOne(x => x.OrganizerProfile)
                .WithOne(x => x.User)
                .HasForeignKey<Organizer>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
